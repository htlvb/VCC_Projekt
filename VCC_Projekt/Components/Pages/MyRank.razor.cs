using Microsoft.AspNetCore.Components;
using VCC_Projekt.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Timers;
using System.Threading.Tasks;

namespace VCC_Projekt.Components.Pages
{
    public partial class MyRank : IDisposable
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Parameter]
        public int EventId { get; set; }

        private Event _event;
        private List<RanglisteResult> _topRankingList = new();
        private RanglisteResult _userRankingEntry;
        private bool showUserEntry = false;
        private bool accessDenied;
        private string accessDeniedMessage = "";
        private string _userId;
        private System.Timers.Timer _refreshTimer;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Get current user ID from authentication state
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated != true)
                {
                    accessDenied = true;
                    accessDeniedMessage = "Sie müssen angemeldet sein, um die Ergebnisse zu sehen.";
                    return;
                }

                // Get the user ID from claims
                _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(_userId))
                {
                    accessDenied = true;
                    accessDeniedMessage = "Benutzer-ID konnte nicht ermittelt werden.";
                    return;
                }

                // Fetch the event
                _event = dbContext.Events.FirstOrDefault(e => e.EventID == EventId);

                if (_event == null)
                {
                    accessDenied = true;
                    accessDeniedMessage = $"Event mit ID {EventId} wurde nicht gefunden.";
                    return;
                }

                // Load the ranking and user position
                LoadRankingAndUserPosition();

                // Initialize refresh timer if access is not denied
                if (!accessDenied)
                {
                    InitializeRefreshTimer();
                }
            }
            catch (Exception ex)
            {
                accessDenied = true;
                accessDeniedMessage = $"Fehler beim Laden der Ergebnisse: {ex.Message}";
            }
        }

        protected override void OnParametersSet()
        {
            if (!accessDenied && (_event == null || _event.EventID != EventId))
            {
                // Reload when the event ID changes
                _ = OnInitializedAsync();
            }
        }

        private void InitializeRefreshTimer()
        {
            // Clean up any existing timer
            _refreshTimer?.Dispose();

            // Create new timer with 5 second interval (same as Dashboard)
            _refreshTimer = new System.Timers.Timer(5000);
            _refreshTimer.Elapsed += async (sender, e) => await RefreshRanking();
            _refreshTimer.AutoReset = true;
            _refreshTimer.Enabled = true;
        }

        private async Task RefreshRanking()
        {
            // Only refresh if not in access denied state
            if (!accessDenied && _event != null)
            {
                await InvokeAsync(() =>
                {
                    try
                    {
                        LoadRankingAndUserPosition();
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error refreshing ranking: {ex.Message}");
                        accessDenied = true;
                        accessDeniedMessage = $"Fehler beim Aktualisieren: {ex.Message}";
                        StopTimer();
                        StateHasChanged();
                    }
                });
            }
            else
            {
                // If we're in access denied state, stop refreshing
                StopTimer();
            }
        }

        private void StopTimer()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Enabled = false;
            }
        }

        private void LoadRankingAndUserPosition()
        {
            try
            {
                // Fetch complete ranking data for the event
                var rankingData = dbContext.Set<RanglisteResult>()
                    .FromSqlRaw("CALL ShowRangliste(@eventId)", new MySqlParameter("@eventId", EventId))
                    .ToList();
                var groups = dbContext.Gruppen
                    .Where(g => g.Event_EventID == EventId)
                    .Select(g => new RanglisteResult
                    {
                        GruppenID = g.GruppenID,
                        Gruppenname = g.Gruppenname,
                        GruppenleiterId = g.GruppenleiterId,
                        Teilnehmertyp = g.Teilnehmertyp,
                        AbgeschlosseneLevel = "", // Default to 0 for participants who haven't completed any levels
                        AnzahlLevel = 0, // Default to 0
                        GesamtFehlversuche = 0, // Default to 0
                        MaxBenötigteZeit = null, // No time data
                        GebrauchteZeit = null // No time data
                    })
                    .ToList();

                var individualUsers = dbContext.UserInGruppe
                    .Where(u => u.Gruppe.Event_EventID == EventId && u.Gruppe.Teilnehmertyp == "Einzelspieler")
                    .Select(u => new RanglisteResult
                    {
                        GruppenID = u.Gruppe.GruppenID,
                        Gruppenname = null, // Individual players have no group name
                        GruppenleiterId = u.User.Id,
                        Teilnehmertyp = "Einzelspieler",
                        AbgeschlosseneLevel = "", // Default to 0
                        AnzahlLevel = 0, // Default to 0
                        GesamtFehlversuche = 0, // Default to 0
                        MaxBenötigteZeit = null, // No time data
                        GebrauchteZeit = null // No time data
                    })
                    .ToList();

                // Combine all participants
                var allParticipants = groups.Concat(individualUsers).ToList();

                var unrankedParticipants = allParticipants
                    .Where(participant => !rankingData.Any(ranking => ranking.GruppenID == participant.GruppenID))
                    .OrderBy(p => p.Gruppenname ?? p.GruppenleiterId.ToString()) // Sort alphabetically
                    .ToList();

                int lastRank = rankingData.Count;
                for (int i = 0; i < unrankedParticipants.Count(); i++)
                {
                    unrankedParticipants[i].Rang = lastRank + i + 1;
                }

                // Get top 10 entries
                _topRankingList = GetFirst10WithFallback(rankingData, unrankedParticipants);

                // Find user's group ID(s)
                var memberGroupIds = dbContext.UserInGruppe
                    .Where(uig => uig.User_UserId == _userId)
                    .Select(uig => uig.Gruppe_GruppenId)
                    .Distinct()
                    .ToList();

                // 2. Get groups where user is the LEADER
                var leaderGroupIds = dbContext.Gruppen
                    .Where(g => g.GruppenleiterId == _userId && g.Event_EventID == EventId)
                    .Select(g => g.GruppenID)
                    .ToList();

                // 3. Combine both lists
                var allUserGroupIds = memberGroupIds.Concat(leaderGroupIds).Distinct().ToList();

                // Find user's ranking entry in either ranked or unranked participants
                _userRankingEntry = allParticipants
                    .FirstOrDefault(r => allUserGroupIds.Contains(r.GruppenID));

                // Check if user entry exists and if it's not already in top 10
                showUserEntry = _userRankingEntry != null && !_topRankingList.Any(r => r.GruppenID == _userRankingEntry.GruppenID);

                if(showUserEntry)
                {
                    _topRankingList.Add(_userRankingEntry);
                }
            }
            catch (Exception ex)
            {
                accessDenied = true;
                accessDeniedMessage = $"Fehler beim Laden der Rangliste: {ex.Message}";
                StopTimer();
            }
        }

        private bool IsUserEntry(RanglisteResult entry)
        {
            if (_userRankingEntry == null)
                return false;

            return entry.GruppenID == _userRankingEntry.GruppenID;
        }

        private static List<T> GetFirst10WithFallback<T>(List<T> primary, List<T> secondary)
        {
            // Take up to 10 from primary
            var result = primary.Take(10).ToList();

            // If we need more, take from secondary
            if (result.Count < 10)
            {
                int needed = 10 - result.Count;
                result.AddRange(secondary.Take(needed));
            }

            // Ensure we return exactly 10 items (if secondary has enough)
            return result.Take(10).ToList();
        }

        public void Dispose()
        {
            // Dispose of the timer when the component is disposed
            if (_refreshTimer != null)
            {
                _refreshTimer.Enabled = false;
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
        }
    }
}