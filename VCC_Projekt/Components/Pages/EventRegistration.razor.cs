using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using static VCC_Projekt.Components.Account.Pages.Register;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventRegistration
    {
        private InputModel Input = new();
        private const string ParticipationTypeSingle = "Einzelspieler";
        private const string ParticipationTypeTeam = "Team";

        private void AddMember()
        {
            if(!Input.NewMemberEmail.ToLower().EndsWith("@htlvb.at"))
            {

            }
            
            if (!string.IsNullOrWhiteSpace(Input.NewMemberEmail) && Input.TeamMembers.Count < 5 && !Input.TeamMembers.Contains(Input.NewMemberEmail))
            {
                Input.TeamMembers.Add(Input.NewMemberEmail);
                Input.NewMemberEmail = string.Empty;
            }
        }

        private void RemoveMember(string email)
        {
            Input.TeamMembers.Remove(email);
        }

        private void HandleSubmit()
        {
            // Logik zum Speichern der Anmeldung
        }

        private class InputModel
        {
            [Required(ErrorMessage = "Bitte wähle eine Teilnahmeart.")]
            [DataType(DataType.Text)]
            public string ParticipationType { get; set; }

            [Required(ErrorMessage = "Bitte Gruppenname vergeben.")]
            [DataType(DataType.Text)]
            [Display(Name = "Gruppenname")]
            public string TeamName { get; set; }

            [Required]
            [AtLeastOneMember(ErrorMessage = "Bitte mindestens ein Gruppenmitglied hinzufügen.")]
            [DataType(DataType.Text)]
            [Display(Name = "Gruppenmitglieder")]
            public List<string> TeamMembers { get; set; } = new();

            [RegularExpression(@"^[a-zA-Z0-9._-]+@[Hh][Tt][Ll][Vv][Bb]\.[Aa][Tt]$",
                    ErrorMessage = "Bitte geben Sie eine gültige @htlvb.at " +
                    "E-Mail-Adresse ein. Erlaubte Sonderzeichen sind ( . - _ )")]
            [UniqueEmail(ErrorMessage = "Diese E-Mail-Adresse existiert bereits")]
            [Display(Name = "E-Mail-Adressen der Teammitglieder")]
            public string NewMemberEmail { get; set; } = "";
        }


        public class AtLeastOneMemberAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var teamMembers = value as List<string>;
                if (teamMembers == null || teamMembers.Count == 0)
                {
                    return new ValidationResult("Bitte mindestens ein Gruppenmitglied hinzufügen.");
                }
                return ValidationResult.Success;
            }
        }
    }
}
