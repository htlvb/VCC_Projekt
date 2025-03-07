using Microsoft.AspNet.Identity;
using VCC_Projekt.Data;
using System.Linq;

namespace VCC_Projekt.Components.Pages
{
    public partial class ViewParticipantsData
    {
        private Event _selectedEvent = new() { EventID = 0 };
        private List<Event> _events = new();
        private List<Participant> _participants = new();
        private string _searchString;

        protected override void OnInitialized()
        {
            _events = dbContext.Events.OrderByDescending(ev => ev.Beginn).ToList();
        }

        private async Task OnEventSelected(Event selectedEvent)
        {
            _selectedEvent = selectedEvent;
            if (_selectedEvent.EventID != 0)
            {
                // Load participants for the selected event
                var groups = dbContext.Gruppen
                    .Where(g => g.Event_EventID == _selectedEvent.EventID)
                    .Select(g => new Participant
                    {
                        Name = g.Gruppenname ?? g.GruppenleiterId,
                        Type = g.Teilnehmertyp,
                        Members = g.UserInGruppe.Select(u => u.User.UserName).ToList()
                    })
                    .ToList();

                var individualUsers = dbContext.UserInGruppe
                    .Where(u => u.Gruppe.Event_EventID == _selectedEvent.EventID && u.Gruppe.Teilnehmertyp == "Einzelspieler")
                    .Select(u => new Participant
                    {
                        Name = u.User.UserName,
                        Type = "Einzelspieler",
                        Members = new List<string>()
                    })
                    .ToList();

                _participants = groups.Concat(individualUsers).ToList();
            }
            else
            {
                _participants.Clear();
            }
        }

        // Search filter logic
        private Func<Participant, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true; // Show all if empty
            var lowerSearch = _searchString.ToLower();
            return x.Name.ToLower().Contains(lowerSearch) ||
                   x.Type.ToLower().Contains(lowerSearch) ||
                   x.Members.Any(m => m.ToLower().Contains(lowerSearch));
        };
    }

    public class Participant
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Members { get; set; }
    }
}