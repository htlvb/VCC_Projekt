using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
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

        private static int eventId;

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
                if (newMember == dbContext.Users.Where(u => u.UserName == Input.Username).Select(u => u.Email).First())
                {
                    addMemberErrors.Add(new ValidationResult("Du bist bereits Mitglieder der Gruppe.", new[] { nameof(Input.NewMemberEmail) }));
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
                var groupManagerUsername = Input.Username;

                if (Input.ParticipationType == ParticipationTypeTeam)
                {
                    try
                    {
                        Gruppe team = new Gruppe
                        {
                            Gruppenname = teamName,
                            Event_EventID = eventId,
                            GruppenleiterId = groupManagerUsername,
                            Teilnehmertyp = Input.ParticipationType
                        };

                        dbContext.Gruppen.Add(team);
                        dbContext.SaveChanges();

                        var teamId = dbContext.Gruppen
                            .Where(u => u.Gruppenname == teamName)
                            .Select(u => u.GruppenID)
                            .FirstOrDefault();

                        UserInGruppe gruppe = new UserInGruppe(Input.Username, teamId);

                        dbContext.UserInGruppe.Add(gruppe);
                        dbContext.SaveChanges();

                        var groupManagerEmail = dbContext.Users.Where(u => u.UserName == groupManagerUsername).Select(u => u.Email).FirstOrDefault();

                        foreach (var memberEmail in Input.TeamMembers)
                        {
                            var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(teamId.ToString()));
                            var invaitationLink = NavigationManager.GetUriWithQueryParameters(
                                    NavigationManager.ToAbsoluteUri($"/Account/Login?groupId={teamId}").AbsoluteUri,
                                    new Dictionary<string, object?>
                                    {
                                        ["inviteToken"] = inviteToken,
                                        ["email"] = memberEmail
                                    }); ;

                            var registerLink = string.Empty;

                            // wenn User noch nicht in DB ist --> zusätzlich registrierlink mitschicken
                            if (!dbContext.Users.Any(u => u.Email == memberEmail))
                            {
                                registerLink = NavigationManager.GetUriWithQueryParameters(
                                    NavigationManager.ToAbsoluteUri($"/Account/Register").AbsoluteUri,
                                    new Dictionary<string, object?>
                                    {
                                        ["inviteToken"] = inviteToken,
                                        ["email"] = memberEmail
                                    });
                            }

                            await EmailSender.SendInvitationLinkAsync(groupManagerUsername, groupManagerEmail, memberEmail, teamName, HtmlEncoder.Default.Encode(invaitationLink), HtmlEncoder.Default.Encode(registerLink));
                        }

                        NavigationManager.NavigateTo($"/signup-event-confirmation?teamname={teamName}&eventId={eventId}");
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
                            GruppenleiterId = groupManagerUsername
                        };

                        dbContext.Gruppen.Add(single);
                        dbContext.SaveChanges();

                        var teamId = dbContext.Gruppen
                                        .Where(u => u.GruppenleiterId == groupManagerUsername && u.Event_EventID == eventId)
                                        .Select(u => u.GruppenID)
                                        .FirstOrDefault();

                        UserInGruppe gruppe = new UserInGruppe(Input.Username, teamId);

                        dbContext.UserInGruppe.Add(gruppe);
                        dbContext.SaveChanges();

                        NavigationManager.NavigateTo($"/signup-event-confirmation?eventId={eventId}");
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

        private partial class InputModel : IValidatableObject
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
                var context = validationContext.GetService<ApplicationDbContext>();

                if (context == null)
                {
                    throw new InvalidOperationException("ApplicationDbContext ist nicht verfügbar.");
                }

                if (ParticipationType != ParticipationTypeSingle && ParticipationType != ParticipationTypeTeam)
                {
                    errors.Add(new ValidationResult("Bitte eine Teilnahmeart eingeben.", new[] { nameof(ParticipationType) }));
                }

                // Abfrage, ob der Gruppenmanager bereits am Event teilnimmt
                var groupIds = context.UserInGruppe
                    .Where(ug => ug.User_UserId.ToUpper() == Username.ToUpper())
                    .Select(ug => ug.Gruppe_GruppenId)
                    .ToList();

                if (groupIds.Any()) // Überprüfen, ob der Benutzer in mindestens einer Gruppe ist
                {
                    // Überprüfen, ob der Benutzer an dem Event teilnimmt
                    var eventIdExists = context.Gruppen
                        .Any(g => groupIds.Contains(g.GruppenID) && g.Event_EventID == eventId);

                    if (eventIdExists)
                    {
                        errors.Add(new ValidationResult("Du nimmst bereits an diesem Event teil.", new[] { nameof(Username) }));
                    }
                }

                if (ParticipationType == ParticipationTypeTeam)
                {
                    if (string.IsNullOrWhiteSpace(TeamName))
                    {
                        errors.Add(new ValidationResult("Bitte einen Gruppenname vergeben.", new[] { nameof(TeamName) }));
                    }

                    else if (context.Gruppen.Where(g => g.Event_EventID == eventId).Any(u => u.Gruppenname.ToUpper() == TeamName.ToString().ToUpper()))
                    {
                        errors.Add(new ValidationResult("Dieser Gruppenname ist bereits vergeben.", new[] { nameof(TeamName) }));
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
