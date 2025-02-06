using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        private bool CanAddMember()
        {
            return Input.TeamMembers.Count < 4 && !string.IsNullOrWhiteSpace(Input.NewMemberEmail);
        }

        private void AddMember()
        {
            //Fehler prüfen
            
            Input.TeamMembers.Add(Input.NewMemberEmail);
            Input.NewMemberEmail = string.Empty;

        }

        private void RemoveMember(string email)
        {
            Input.TeamMembers.Remove(email);
        }

        private async void HandleSubmit()
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(Input);
                bool isValid = Validator.TryValidateObject(Input, validationContext, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        Console.WriteLine($"Validation Error: {validationResult.ErrorMessage}");
                    }
                    return;
                }

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

                    var teamId = dbContext.Gruppen.Where(u => u.Gruppenname == teamName).Select(u => u.GruppenID);

                    foreach (var memberEmail in Input.TeamMembers)
                    {
                        var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(teamId.ToString()));

                        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                            NavigationManager.ToAbsoluteUri("/Account/Register").AbsoluteUri,
                            new Dictionary<string, object?> { ["inviteToken"] = inviteToken, ["email"] = memberEmail });

                        await EmailSender.SendInvitationLinkAsync(groupManager, memberEmail, teamName, HtmlEncoder.Default.Encode(callbackUrl));
                    }

                    NavigationManager.NavigateTo("/signup-event-confirmation");

                }

                else
                {
                    try
                    {
                        Gruppe single = new Gruppe();

                        //Gruppenname = Null (wegen Einzelspieler, mit Spalt besprochen)

                        single.Event_EventID = eventId;
                        single.Teilnehmertyp = Input.ParticipationType;
                        single.GruppenleiterId = groupManager;

                        dbContext.Gruppen.Add(single);
                        dbContext.SaveChanges();

                        NavigationManager.NavigateTo("/signup-event-confirmation");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during submission: {ex.Message}");
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

        private class InputModel : IValidatableObject
        {
            [DataType(DataType.Text)]
            public string ParticipationType { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Gruppenname")]
            public string TeamName { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Gruppenmitglieder")]
            public List<string> TeamMembers { get; set; } = new();

            [Display(Name = "E-Mail-Adressen der Teammitglieder")]
            public string NewMemberEmail { get; set; } = "";

            [DataType(DataType.Text)]
            [Display(Name = "Benutzername")]
            public string Username { get; set; } = "";


            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var errors = new List<ValidationResult>();

                if (ParticipationType != ParticipationTypeSingle && ParticipationType != ParticipationTypeTeam)
                {
                    errors.Add(new ValidationResult("Bitte eine Teilnahmeart eingeben.", new[] { nameof(ParticipationType) }));
                }

                if (ParticipationType == ParticipationTypeSingle)
                {
                    //Keine Fehler
                }

                if (ParticipationType == ParticipationTypeTeam)
                {
                    string newMember = NewMemberEmail;

                    if (string.IsNullOrWhiteSpace(TeamName))
                    {
                        errors.Add(new ValidationResult("Bitte einen Gruppenname vergeben.", new[] { nameof(TeamName) }));
                    }

                    if (TeamMembers.Count == 0)
                    {
                        errors.Add(new ValidationResult("Bitte mindestens ein Gruppenmitglied hinzufügen.", new[] { nameof(NewMemberEmail) }));
                    }

                    if (!string.IsNullOrWhiteSpace(newMember))
                    {
                        if (!Regex.IsMatch(newMember, @"(?i)^.+@htlvb\.at$"))
                        {
                            errors.Add(new ValidationResult("Bitte eine gültige @htlvb.at E-Mail-Adresse eingeben.", new[] { nameof(NewMemberEmail) }));
                        }

                        else if (TeamMembers.Contains(newMember))
                        {
                            errors.Add(new ValidationResult("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", new[] { nameof(NewMemberEmail) }));
                        }

                        else if (TeamMembers.Count >= 4)
                        {
                            errors.Add(new ValidationResult("Die maximale Gruppengröße von 4 Teilnehmern wurde bereits erreicht.", new[] { nameof(NewMemberEmail) }));
                        }
                    }
                }

                return errors;
            }

        }
    }
}
