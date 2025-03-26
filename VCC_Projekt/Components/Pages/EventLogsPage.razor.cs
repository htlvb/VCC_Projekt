namespace VCC_Projekt.Components.Pages
{
    public partial class EventLogsPage
    {
        private List<EventLog> eventLogs = new();
        private string _searchString = "";


        // Quick filter function that works across multiple columns
        private Func<EventLog, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Tabellenname?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.Beschreibung?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.LogKat.Beschreibung?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.EventLogID.ToString().Contains(_searchString))
                return true;

            if (x.Zeit.ToString("dd.MM.yyyy HH:mm:ss").Contains(_searchString))
                return true;

            return false;
        };

        protected override void OnInitialized()
        {
            // Use a projection to get both the log and category information
            eventLogs = dbContext.EventLogs.Include(e => e.LogKat).ToList();
        }
    }
}
