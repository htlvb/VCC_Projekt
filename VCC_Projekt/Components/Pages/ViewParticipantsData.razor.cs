using VCC_Projekt.Data;

namespace VCC_Projekt.Components.Pages
{
    public partial class  ViewParticipantsData
    {
        private Event _selectedEvent = new() { EventID = 0 };
        private List<Event> _events = new();
        protected override void OnInitialized()
        {
            _events = dbContext.Events.OrderByDescending(ev => ev.Beginn).ToList();
        }

        private async Task OnEventSelected(Event selectedEvent)
        {
            _selectedEvent = selectedEvent;
            //_levels = _selectedEvent.EventID != 0
            //    ? await LoadLevelsAsync()
            //    : new List<LevelViewModel>();
        }
    }
}
