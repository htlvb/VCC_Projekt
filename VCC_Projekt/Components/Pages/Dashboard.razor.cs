using Microsoft.AspNetCore.Components;
using VCC_Projekt.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Timers;
using System.Threading.Tasks;

namespace VCC_Projekt.Components.Pages
{
    public partial class Dashboard : IDisposable
    {
        private List<Event> _events = new();
        private Event _selectedEvent;
        private List<RanglisteResult> _rankingList = new();
        private List<Participants> _participants = new();
        private System.Timers.Timer _refreshTimer;
        private bool isRanking = false; // New variable to check if ranking is populated

        protected override void OnInitialized()
        {
            try
            {
                var now = DateTime.Now;
                _events = dbContext.Events
                    .Where(x => x.Beginn <= now && x.Beginn.AddMinutes(x.Dauer) > now)
                    .OrderByDescending(ev => ev.Beginn)
                    .ToList();

                // Automatically select the first event if there are multiple events
                if (_events.Count > 0)
                {
                    _selectedEvent = _events[0];
                    LoadRanking(_selectedEvent.EventID); // Load ranking for the default selected event
                }

                // Initialize and start the timer to refresh the ranking every 5 seconds
                _refreshTimer = new System.Timers.Timer(5000); // 5000 milliseconds = 5 seconds
                _refreshTimer.Elapsed += RefreshRanking;
                _refreshTimer.AutoReset = true;
                _refreshTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
                _events = new List<Event>();
            }
        }

        private async void RefreshRanking(object sender, ElapsedEventArgs e)
        {
            if (_selectedEvent != null)
            {
                await InvokeAsync(() =>
                {
                    LoadRanking(_selectedEvent.EventID);
                    StateHasChanged(); // Notify the component that the state has changed
                });
            }
        }

        private async Task OnEventSelected(Event selectedEvent)
        {
            _selectedEvent = selectedEvent; // Store the selected event
            LoadRanking(selectedEvent.EventID); // Load ranking for the newly selected event
        }

        private void LoadRanking(int eventId)
        {
            try
            {
                // Fetch ranking data for the selected event
                _rankingList = dbContext.Set<RanglisteResult>()
                    .FromSqlRaw("CALL ShowRangliste(@eventId)", new MySqlParameter("@eventId", eventId))
                    .ToList();

                // Check if ranking is null or empty
                isRanking = _rankingList != null && _rankingList.Any();

                // If ranking is null or empty, populate it with participants
                if (!isRanking)
                {
                    var groups = dbContext.Gruppen
                        .Where(g => g.Event_EventID == eventId)
                        .Select(g => new RanglisteResult
                        {
                            GruppenID = g.GruppenID,
                            Gruppenname = g.Gruppenname,
                            GruppenleiterId = g.GruppenleiterId,
                            Teilnehmertyp = g.Teilnehmertyp,
                            AbgeschlosseneLevel = "", // No levels completed
                            AnzahlLevel = 0, // No levels completed
                            GesamtFehlversuche = 0, // No attempts
                            MaxBenötigteZeit = null, // No time data
                            GebrauchteZeit = null // No time data
                        })
                        .ToList();

                    var individualUsers = dbContext.UserInGruppe
                        .Where(u => u.Gruppe.Event_EventID == eventId && u.Gruppe.Teilnehmertyp == "Einzelspieler")
                        .Select(u => new RanglisteResult
                        {
                            GruppenID = u.Gruppe.GruppenID,
                            Gruppenname = u.User.UserName,
                            GruppenleiterId = u.User.Id,
                            Teilnehmertyp = "Einzelspieler",
                            AbgeschlosseneLevel = "", // No levels completed
                            AnzahlLevel = 0, // No levels completed
                            GesamtFehlversuche = 0, // No attempts
                            MaxBenötigteZeit = null, // No time data
                            GebrauchteZeit = null // No time data
                        })
                        .ToList();

                    _rankingList = groups.Concat(individualUsers).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ranking: {ex.Message}");
                _rankingList = new List<RanglisteResult>();
                isRanking = false; // Reset the flag in case of an error
            }
        }

        public void Dispose()
        {
            // Dispose of the timer when the component is disposed
            _refreshTimer?.Dispose();
        }
    }
}