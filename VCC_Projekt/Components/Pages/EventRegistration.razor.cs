using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

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

        private List<ValidationResult> addMemberErrors = new List<ValidationResult>();

        private string userEmail;
        private int eventId;

        protected override void OnInitialized()
        {
            var uri = new Uri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue, out int parsedEventId))
            {
                eventId = parsedEventId;
            }
        }

        private bool CanAddMember()
        {
            return Input.TeamMembers.Count < 4 && !string.IsNullOrWhiteSpace(Input.NewMemberEmail);
        }

        private void AddMember()
        {
            addMemberErrors.Clear();
            var newMember = Input.NewMemberEmail;

            if (!string.IsNullOrWhiteSpace(newMember))
            {
                if(newMember == dbContext.Users.Where(u => u.UserName == Input.Username).Select(u => u.Email).First())
                {
                    addMemberErrors.Add(new ValidationResult("Du bist bereits Mitglieder der Gruppe.", new[] {nameof(Input.NewMemberEmail)}));
                }

                else if (!Regex.IsMatch(newMember, @"(?i)^.+@htlvb\.at$"))
                {
                    addMemberErrors.Add(new ValidationResult("Bitte eine gültige @htlvb.at E-Mail-Adresse eingeben.", new[] { nameof(Input.NewMemberEmail) }));
                }
                else if (Input.TeamMembers.Contains(newMember))
                {
                    addMemberErrors.Add(new ValidationResult("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", new[] { nameof(Input.NewMemberEmail) }));
                }
                else if (Input.TeamMembers.Count >= 4)
                {
                    addMemberErrors.Add(new ValidationResult("Die maximale Gruppengröße von 4 Teilnehmern wurde bereits erreicht.", new[] { nameof(Input.NewMemberEmail) }));
                }
            }

            if (addMemberErrors.Count == 0)
            {
                Input.TeamMembers.Add(newMember);
                Input.NewMemberEmail = string.Empty;
            }

            StateHasChanged();
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

                var teamName = Input.TeamName;
                var groupManager = Input.Username;

                if (Input.ParticipationType == ParticipationTypeTeam)
                {
                    try
                    {
                        Gruppe team = new Gruppe
                        {
                            Gruppenname = teamName,
                            Event_EventID = eventId,
                            GruppenleiterId = groupManager,
                            Teilnehmertyp = Input.ParticipationType
                        };

                        dbContext.Gruppen.Add(team);
                        dbContext.SaveChanges();

                        var teamId = dbContext.Gruppen
                            .Where(u => u.Gruppenname == teamName)
                            .Select(u => u.GruppenID)
                            .FirstOrDefault();

                        foreach (var memberEmail in Input.TeamMembers)
                        {
                            var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(teamId.ToString()));

                            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                                NavigationManager.ToAbsoluteUri("/Account/Register").AbsoluteUri,
                                new Dictionary<string, object?>
                                {
                                    ["inviteToken"] = inviteToken,
                                    ["email"] = memberEmail
                                });

                            await EmailSender.SendInvitationLinkAsync(groupManager, memberEmail, teamName, HtmlEncoder.Default.Encode(callbackUrl));
                        }

                        NavigationManager.NavigateTo("/signup-event-confirmation");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Team Registration: {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        Gruppe single = new Gruppe
                        {
                            Event_EventID = eventId,
                            Teilnehmertyp = Input.ParticipationType,
                            GruppenleiterId = groupManager
                        };

                        dbContext.Gruppen.Add(single);
                        dbContext.SaveChanges();

                        NavigationManager.NavigateTo("/signup-event-confirmation");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Single Registration: {ex.Message}");
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

                if (ParticipationType == ParticipationTypeTeam)
                {
                    if (string.IsNullOrWhiteSpace(TeamName))
                    {
                        errors.Add(new ValidationResult("Bitte einen Gruppenname vergeben.", new[] { nameof(TeamName) }));
                    }

                    if (TeamMembers.Count == 0)
                    {
                        errors.Add(new ValidationResult("Bitte mindestens ein Gruppenmitglied hinzufügen.", new[] { nameof(NewMemberEmail) }));
                    }
                }

                return errors;
            }
        }
    }
}
