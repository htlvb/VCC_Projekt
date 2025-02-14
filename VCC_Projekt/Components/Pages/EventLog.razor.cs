using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Net.Http.Json;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventLog
    {

        private IEnumerable<EventLogViewModel> EventLogs = new List<EventLogViewModel>();
        private string _searchString;
       

        // Quick filter - search across multiple columns
        private Func<EventLogViewModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Tabellename?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.Beschreibung?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.CategoryDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if ($"{x.EventLogID}".Contains(_searchString))
                return true;

            if (x.Zeit.ToString("yyyy-MM-dd HH:mm:ss").Contains(_searchString))
                return true;

            return false;
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Get both event logs and categories
                var eventLogs = dbContext.EventLogs.Include(u => u.LogKat).ToList();

                if (eventLogs != null && categories != null)
                {
                    // Join the data to create view models
                    EventLogs = eventLogs.Select(log => new EventLogViewModel
                    {
                        EventLogID = log.EventLogID,
                        Tabellename = log.Tabellename,
                        Beschreibung = log.Beschreibung,
                        Zeit = log.Zeit,
                        LogKategorie_KatID = log.LogKategorie_KatID,
                        CategoryDescription = categories
                                .FirstOrDefault(c => c.KatID == log.LogKategorie_KatID)
                                ?.Beschreibung ?? "Unknown"
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Add proper error handling here
                Console.WriteLine($"Error loading event logs: {ex.Message}");
            }
        }
    }

    public class LogKategorie
    {
        public int KatID { get; set; }
        public string Beschreibung { get; set; }
    }

    public class EventLog
    {
        public int EventLogID { get; set; }
        public string Tabellename { get; set; }
        public string Beschreibung { get; set; }
        public DateTime Zeit { get; set; }
        public int LogKategorie_KatID { get; set; }
    }
}