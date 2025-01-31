using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using static VCC_Projekt.Components.Account.Pages.Register;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventRegistration
    {
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        private const string ParticipationTypeSingle = "Einzelspieler";
        private const string ParticipationTypeTeam = "Team";

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private void AddMember()
        {
            if (!Input.NewMemberEmail.ToLower().EndsWith("@htlvb.at"))
            {
                //Fehler
            }

            if (!string.IsNullOrWhiteSpace(Input.NewMemberEmail) && Input.TeamMembers.Count < 4 && !Input.TeamMembers.Contains(Input.NewMemberEmail))
            {
                Input.TeamMembers.Add(Input.NewMemberEmail);
                Input.NewMemberEmail = string.Empty;
            }
        }

        private void RemoveMember(string email)
        {
            Input.TeamMembers.Remove(email);
        }

        private async void HandleSubmit()
        {
            if (Input.ParticipationType == ParticipationTypeTeam)
            {
                //Datenbankeintrag in Gruppentabelle

                var teamId = 1; // Die ID des Teams
                var teamName = "hallo";
                var groupManager = Input.Username;

                // Einladung für jedes Teammitglied versenden
                foreach (var memberEmail in Input.TeamMembers)
                {
                    var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(teamId.ToString()));

                    var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                        NavigationManager.ToAbsoluteUri("/Account/Register").AbsoluteUri,
                        new Dictionary<string, object?> { ["inviteToken"] = inviteToken, ["email"] = memberEmail });

                    await EmailSender.SendInvitationLinkAsync(groupManager, memberEmail, teamName, HtmlEncoder.Default.Encode(callbackUrl));

                    //Logger.LogInformation($"Einladung an {memberEmail} gesendet.");

                }
            }

            if (Input.ParticipationType == ParticipationTypeSingle)
            {

            }
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                Input.Username = user.Identity.Name; // Benutzername abrufen
            }
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

            [DataType(DataType.Text)]
            [Display(Name = "Benutzername")]
            public string Username { get; set; } = "";
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
