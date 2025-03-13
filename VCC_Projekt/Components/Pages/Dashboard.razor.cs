using Microsoft.AspNetCore.Components;
using VCC_Projekt.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace VCC_Projekt.Components.Pages
{
    public partial class Dashboard
    {
        private List<Event> _events = new();
        private Event _selectedEvent;
        private List<RanglisteResult> _rankingList = new();
        private string _searchText = string.Empty;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
                _events = new List<Event>();
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
                    .FromSqlRaw("CALL ShowRangliste(@eventId)", new MySqlParameter("@eventId", 1))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ranking: {ex.Message}");
                _rankingList = new List<RanglisteResult>();
            }
        }
        

        private IEnumerable<RanglisteResult> FilteredRankingList =>
            _rankingList.Where(r =>
                r.Gruppenname?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.GruppenleiterId?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.Teilnehmertyp.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                r.AbgeschlosseneLevel.Contains(_searchText, StringComparison.OrdinalIgnoreCase)
            ).ToList();
    }
}