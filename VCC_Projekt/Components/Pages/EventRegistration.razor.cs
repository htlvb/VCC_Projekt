using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventRegistration
    {
        private RegistrationData registrationData = new();
        private string newMemberEmail;
        private const string ParticipationTypeSingle = "Einzelspieler";
        private const string ParticipationTypeTeam = "Team";

        private void AddMember()
        {
            if (!string.IsNullOrWhiteSpace(newMemberEmail) && registrationData.TeamMembers.Count < 5)
            {
                registrationData.TeamMembers.Add(newMemberEmail);
                newMemberEmail = string.Empty;
            }
        }

        private void RemoveMember(string email)
        {
            registrationData.TeamMembers.Remove(email);
        }

        private void HandleSubmit()
        {
            // Logik zum Speichern der Anmeldung
        }

        private class RegistrationData
        {
            [Required(ErrorMessage = "Bitte wähle eine Teilnahmeart.")]
            public string ParticipationType { get; set; }

            public string TeamName { get; set; }
            public List<string> TeamMembers { get; set; } = new();
        }



    }
}
