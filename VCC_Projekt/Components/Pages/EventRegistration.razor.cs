using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using VCC_Projekt.Components.Account.Pages.Manage;

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

        private static int eventId;

        protected override async Task OnInitializedAsync()
        {
            var uri = new Uri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue, out int parsedEventId))
            {
                eventId = parsedEventId;
            }

            Input.Snackbar = Snackbar;

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                Input.Username = user.Identity.Name;
            }
        }

        private bool CanAddMember()
        {
            return Input.TeamMembers.Count < 4 && !string.IsNullOrWhiteSpace(Input.NewMemberEmail);
        }

        private void AddMember()
        {
            var newMember = Input.NewMemberEmail;

            if (!string.IsNullOrWhiteSpace(newMember))
            {
                var username = dbContext.Users.Where(u => u.NormalizedEmail == newMember.Normalize()).Select(u => u.NormalizedUserName).FirstOrDefault();

                if (username != null)
                {
                    var groupIds = dbContext.UserInGruppe
                        .Where(ug => ug.User_UserId.ToUpper() == username)
                        .Select(ug => ug.Gruppe_GruppenId)
                        .ToList();

                    if (groupIds.Any())
                    {
                        var eventIdExists = dbContext.Gruppen
                            .Any(g => groupIds.Contains(g.GruppenID) && g.Event_EventID == eventId);

                        if (eventIdExists)
                        {
                            ShowSnackbar("Diese Person nimmt bereits am Event teil. Um sie/ihn trotzdem ins Team zu holen, muss sie/er sich zuerst wieder abmelden.", Severity.Error);
                            return;
                        }
                    }
                }

                if (newMember == dbContext.Users.Where(u => u.UserName == Input.Username).Select(u => u.Email).First())
                {
                    ShowSnackbar("Du bist bereits Mitglieder der Gruppe.", Severity.Error);
                    return;
                }
                else if (!Regex.IsMatch(newMember, @"(?i)^.+@htlvb\.at$"))
                {
                    ShowSnackbar("Bitte eine gültige @htlvb.at E-Mail-Adresse eingeben.", Severity.Error);
                    return;
                }
                else if (Input.TeamMembers.Contains(newMember))
                {
                    ShowSnackbar("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", Severity.Error);
                    return;
                }
                else if (Input.TeamMembers.Count >= 4)
                {
                    ShowSnackbar("Die maximale Gruppengröße von 4 Teilnehmern wurde bereits erreicht.", Severity.Error);
                    return;
                }
            }

            Input.TeamMembers.Add(newMember);
            ShowSnackbar($"{newMember} wurde erfolgreich zur Gruppe hinzugefügt.", Severity.Success);
            Input.NewMemberEmail = string.Empty;

            StateHasChanged();
        }

        private void RemoveMember(string email)
        {
            Input.TeamMembers.Remove(email);
            ShowSnackbar($"{email} wurde erfolgreich aus der Gruppe entfernt.", Severity.Success);
        }

        private async void HandleSubmit()
        {
            try
            {
                Input.dbContext = dbContext;
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

                        var groupId = dbContext.Gruppen
                            .Where(u => u.Gruppenname == teamName && u.Event_EventID == eventId)
                            .Select(u => u.GruppenID)
                            .FirstOrDefault();

                        UserInGruppe gruppe = new UserInGruppe(Input.Username, groupId);

                        dbContext.UserInGruppe.Add(gruppe);
                        dbContext.SaveChanges();

                        var groupManagerEmail = dbContext.Users.Where(u => u.UserName == groupManagerUsername).Select(u => u.Email).FirstOrDefault();

                        foreach (var memberEmail in Input.TeamMembers)
                        {
                            var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(groupId.ToString()));
                            var invaitationLink = NavigationManager.GetUriWithQueryParameters(
                                    NavigationManager.ToAbsoluteUri($"/Account/Login?groupId={groupId}").AbsoluteUri,
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

                            EingeladeneUserInGruppe invitedMember = new EingeladeneUserInGruppe(memberEmail, groupId);
                            dbContext.EingeladeneUserInGruppe.Add(invitedMember);
                            dbContext.SaveChanges();
                        }

                        var eventName = dbContext.Events.Where(e => e.EventID == eventId).Select(e => e.Bezeichnung).FirstOrDefault();

                        ShowSnackbar($"Du hast dich erfolgreich für den Wettbewerb '{eventName}' in der Gruppe '{teamName}' angemeldet.", Severity.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowSnackbar($"Fehler beim Anmelden für den Wettbewerb.", Severity.Error);

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

                        var eventName = dbContext.Events.Where(e => e.EventID == eventId).Select(e => e.Bezeichnung).FirstOrDefault();

                        ShowSnackbar($"Du hast dich erfolgreich für den Wettbewerb '{eventName}' angemeldet.", Severity.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowSnackbar($"Fehler beim Anmelden für den Wettbewerb.", Severity.Error);

                        Console.WriteLine($"Error in Single Registration: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSnackbar($"Fehler beim Anmelden für den Wettbewerb.", Severity.Error);

                Console.WriteLine($"Error during submission: {ex.Message}");
            }
        }


        public void ShowSnackbar(string message, Severity severity)
                => Snackbar.Add(message, severity);

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

            public ApplicationDbContext dbContext { get; set; }

            public ISnackbar Snackbar { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var errors = new List<ValidationResult>();
                var context = validationContext.GetService<ApplicationDbContext>();

                bool isValid = true;

                if (dbContext == null)
                {
                    dbContext = context;
                    // throw new InvalidOperationException("ApplicationDbContext ist nicht verfügbar.");
                }

                if (ParticipationType != ParticipationTypeSingle && ParticipationType != ParticipationTypeTeam)
                {
                    Snackbar?.Add("Bitte eine Teilnahmeart auswählen.", Severity.Error);
                    isValid = false;
                }

                // Abfrage, ob der Gruppenmanager bereits am Event teilnimmt
                var groupIds = dbContext.UserInGruppe
                    .Where(ug => ug.User_UserId.ToUpper() == Username.ToUpper())
                    .Select(ug => ug.Gruppe_GruppenId)
                    .ToList();

                if (groupIds.Any()) // Überprüfen, ob der Benutzer in mindestens einer Gruppe ist
                {
                    // Überprüfen, ob der Benutzer an dem Event teilnimmt
                    var eventIdExists = dbContext.Gruppen
                        .Any(g => groupIds.Contains(g.GruppenID) && g.Event_EventID == eventId);

                    if (eventIdExists)
                    {
                        Snackbar?.Add("Du nimmst bereits an diesem Event teil.", Severity.Error);
                        isValid = false;
                    }
                }

                if (ParticipationType == ParticipationTypeTeam)
                {
                    if (string.IsNullOrWhiteSpace(TeamName))
                    {
                        Snackbar?.Add("Bitte einen Gruppenname vergeben.", Severity.Error);
                        isValid = false;
                    }

                    else if (dbContext.Gruppen.Where(g => g.Event_EventID == eventId).Any(u => u.Gruppenname.ToUpper() == TeamName.ToString().ToUpper()))
                    {
                        Snackbar?.Add("Dieser Gruppenname ist bereits vergeben.", Severity.Error);
                        isValid = false;
                    }

                    if (TeamMembers.Count == 0)
                    {
                        Snackbar?.Add("Bitte mindestens ein Gruppenmitglied hinzufügen.", Severity.Error);
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    errors.Add(new ValidationResult("Snackbar-Fehler wurden angezeigt."));
                }

                return errors;
            }
        }
    }
}
