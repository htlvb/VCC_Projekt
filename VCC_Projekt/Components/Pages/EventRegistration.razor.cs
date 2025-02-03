using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using static VCC_Projekt.Components.Account.Pages.Register;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventRegistration
    {
        [SupplyParameterFromForm]

        private InputModel Input { get; set; } = new();

        private const string ParticipationTypeSingle = "Einzelspieler";
        private const string ParticipationTypeTeam = "Team";

        string error = string.Empty;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private void AddMember()
        {
            string newMember = Input.NewMemberEmail;

            if (string.IsNullOrWhiteSpace(newMember))
            {
                error = "Bitte geben Sie eine E-Mail-Adresse ein.";
                Console.WriteLine(error);
                return;
            }

            if (!Regex.IsMatch(newMember, @"(?i)^.+@htlvb\.at$"))
            {
                error = "Bitte geben Sie eine gültige @htlvb.at E-Mail-Adresse ein.";
                Console.WriteLine(error);
                return;
            }

            if (Input.TeamMembers.Contains(newMember))
            {
                error = "Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.";
                Console.WriteLine(error);
                return;
            }

            if (Input.TeamMembers.Count >= 4)
            {
                error = "Die maximale Gruppengröße von 4 Teilnehmern wurde bereits erreicht.";
                Console.WriteLine(error);
                return;
            }

            Input.TeamMembers.Add(Input.NewMemberEmail);
            Input.NewMemberEmail = string.Empty;
        }

        private void RemoveMember(string email)
        {
            Input.TeamMembers.Remove(email);
        }

        private async void HandleSubmit()
        {
            var teamName = Input.TeamName; // Teamname
            var groupManager = Input.Username;
            var eventId = 1;

            if (Input.ParticipationType == ParticipationTypeTeam)
            {

                try
                {
                    Gruppe team = new Gruppe();

                    team.Gruppenname = teamName;
                    team.Event_EventID = eventId;
                    team.GruppenleiterId = groupManager;
                    team.Teilnehmertyp = Input.ParticipationType; 

                    dbContext.Gruppen.Add(team);
                    dbContext.SaveChanges();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
                //Datenbankeintrag in Gruppentabelle

                var teamId = 1; // Die ID des Teams


                foreach (var memberEmail in Input.TeamMembers)
                {
                    var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(teamId.ToString()));

                    var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                        NavigationManager.ToAbsoluteUri("/Account/Register").AbsoluteUri,
                        new Dictionary<string, object?> { ["inviteToken"] = inviteToken, ["email"] = memberEmail });

                     await EmailSender.SendInvitationLinkAsync(groupManager, memberEmail, teamName, HtmlEncoder.Default.Encode(callbackUrl));
                }
            }

            else
            {
                //Datenbankeintrag mit benutzername als teamname 
                try
                {

                    Gruppe single = new Gruppe();

                    single.Gruppenname = groupManager;
                    single.Event_EventID = eventId;
                    single.GruppenleiterId = groupManager;
                    single.Teilnehmertyp = Input.ParticipationType;

                    dbContext.Gruppen.Add(single);
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                Input.Username = user.Identity.Name;
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

            [RegularExpression(@"(?i)^.+@htlvb\.at$",
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
