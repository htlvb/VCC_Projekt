using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Text.RegularExpressions;
using VCC_Projekt.Data;
using VCC_Projekt.Components.Account.Pages.Manage;
using MudBlazor;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventsOverview
    {
        private List<Event> events;
        private List<Event> filteredEvents = new List<Event>();
        private List<Gruppe> userGroups = new List<Gruppe>();
        private static List<string> invitedUsers = new List<string>();
        private static string usernameLoggedInUser;

        private List<ValidationResult> addMemberErrors = new List<ValidationResult>();
        private List<RanglisteResult> Ranglist { get; set; }

        private bool isPastEventsActive = false;
        private bool isUpcomingEventsActive = false;

        private bool isAddingMember;

        public static int selectedEventId;

        private MemberModel newMember = new MemberModel();

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.Identity.IsAuthenticated)
            {
                usernameLoggedInUser = user.Identity.Name;

                if (!string.IsNullOrEmpty(usernameLoggedInUser))
                {
                    userGroups = await dbContext.Gruppen
                        .Where(g => g.UserInGruppe.Any(u => u.User_UserId == usernameLoggedInUser))
                        .Include(g => g.UserInGruppe)
                        .ToListAsync();

                    events = await dbContext.Gruppen
                        .Where(g => g.UserInGruppe.Any(u => u.User_UserId == usernameLoggedInUser))
                        .Select(g => g.Event)
                        .ToListAsync();

                    ShowUpcomingEvents();
                }
            }
        }

        private void ShowUpcomingEvents()
        {
            filteredEvents = events.Where(e => e.Beginn.AddMinutes(e.Dauer) >= DateTime.Now).ToList();
            isPastEventsActive = false;
            isUpcomingEventsActive = true;
        }

        private void ShowPastEvents()
        {
            filteredEvents = events.Where(e => e.Beginn.AddMinutes(e.Dauer) < DateTime.Now).ToList();
            isPastEventsActive = true;
            isUpcomingEventsActive = false;
        }

        private async Task Unregister(int eventId)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                usernameLoggedInUser = user.Identity.Name;
                if (string.IsNullOrEmpty(usernameLoggedInUser))
                {
                    Console.WriteLine("Fehler: Benutzer-ID konnte nicht ermittelt werden.");
                    return;
                }

                var userGroupEntry = await dbContext.UserInGruppe
                    .FirstOrDefaultAsync(uig => uig.User_UserId == usernameLoggedInUser &&
                                                dbContext.Gruppen.Any(g => g.GruppenID == uig.Gruppe_GruppenId && g.Event_EventID == eventId));

                if (userGroupEntry == null)
                {
                    Console.WriteLine($"Benutzer {usernameLoggedInUser} ist für Event {eventId} in keiner Gruppe.");
                    return;
                }

                int groupId = userGroupEntry.Gruppe_GruppenId;

                dbContext.UserInGruppe.Remove(userGroupEntry);
                await dbContext.SaveChangesAsync();

                bool isGroupEmpty = !await dbContext.UserInGruppe.AnyAsync(uig => uig.Gruppe_GruppenId == groupId);

                if (isGroupEmpty)
                {
                    var groupToDelete = await dbContext.Gruppen.FindAsync(groupId);
                    if (groupToDelete != null)
                    {
                        dbContext.Gruppen.Remove(groupToDelete);
                        await dbContext.SaveChangesAsync();
                    }
                }

                events = await dbContext.Gruppen
                    .Where(g => g.UserInGruppe.Any(u => u.User_UserId == usernameLoggedInUser))
                    .Select(g => g.Event)
                    .ToListAsync();
            }

            else
            {
                Console.WriteLine("Benutzer ist nicht authentifiziert.");
            }
        }

        private async Task RemoveMember(int groupId, string memberId)
        {
            var memberEntry = await dbContext.UserInGruppe
                .FirstOrDefaultAsync(uig => uig.Gruppe_GruppenId == groupId && uig.User_UserId == memberId);

            if (memberEntry != null)
            {
                dbContext.UserInGruppe.Remove(memberEntry);
                await dbContext.SaveChangesAsync();

                events = await dbContext.Gruppen
                    .Where(g => g.UserInGruppe.Any(u => u.User_UserId == usernameLoggedInUser))
                    .Select(g => g.Event)
                    .ToListAsync();
            }
        }

        private async Task JoinEvent(int eventId)
        {
            NavigationManager.NavigateTo($"/participation/{eventId}");
        }

        private async Task AddMember(int groupId, int eventId, string newMemberEmail)
        {
            try
            {
                var groupManagerEmail = dbContext.Users.Where(u => u.UserName == usernameLoggedInUser).Select(u => u.Email).FirstOrDefault();

                var inviteToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(groupId.ToString()));
                var invaitationLink = NavigationManager.GetUriWithQueryParameters(
                        NavigationManager.ToAbsoluteUri($"/Account/Login?groupId={groupId}").AbsoluteUri,
                        new Dictionary<string, object?>
                        {
                            ["inviteToken"] = inviteToken,
                            ["email"] = newMemberEmail
                        }); ;

                var registerLink = string.Empty;

                // wenn User noch nicht in DB ist --> zusätzlich registrierlink mitschicken
                if (!dbContext.Users.Any(u => u.Email == newMemberEmail))
                {
                    registerLink = NavigationManager.GetUriWithQueryParameters(
                        NavigationManager.ToAbsoluteUri($"/Account/Register").AbsoluteUri,
                        new Dictionary<string, object?>
                        {
                            ["inviteToken"] = inviteToken,
                            ["email"] = newMemberEmail
                        });
                }

                var teamName = dbContext.Gruppen.Where(g => g.GruppenID == groupId).Select(g => g.Gruppenname).FirstOrDefault();

                await EmailSender.SendInvitationLinkAsync(usernameLoggedInUser, groupManagerEmail, newMemberEmail, teamName, HtmlEncoder.Default.Encode(invaitationLink), HtmlEncoder.Default.Encode(registerLink));

                EingeladeneUserInGruppe invitedMember = new EingeladeneUserInGruppe(newMemberEmail, groupId);
                dbContext.EingeladeneUserInGruppe.Add(invitedMember);
                dbContext.SaveChanges();

                invitedUsers.Add(newMemberEmail);

                isAddingMember = false; // Schließen Sie das Eingabefeld nach dem Hinzufügen
                newMember.Email = string.Empty; // Eingabefeld zurücksetzen
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error in Registration: {ex.Message}");
            }

        }

        private void ShowAddMemberInput()
        {
            isAddingMember = true;
        }

        public class MemberModel : IValidatableObject
        {
            [Required(ErrorMessage = "E-Mail-Adresse ist erforderlich.")]
            public string Email { get; set; }

            public ApplicationDbContext dbContext { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var errors = new List<ValidationResult>();
                var context = validationContext.GetService<ApplicationDbContext>();

                if (dbContext == null)
                {
                    dbContext = context;
                }

                if (!string.IsNullOrWhiteSpace(Email))
                {
                    var usernameNewMember = dbContext.Users.Where(u => u.NormalizedEmail == Email.Normalize()).Select(u => u.NormalizedUserName).FirstOrDefault();

                    if (Email == dbContext.Users.Where(u => u.UserName == usernameLoggedInUser).Select(u => u.Email).First())
                    {
                        errors.Add(new ValidationResult("Du bist bereits Mitglieder der Gruppe.", new[] { nameof(Email) }));
                    }

                    else if (!Regex.IsMatch(Email, @"(?i)^.+@htlvb\.at$"))
                    {
                        errors.Add(new ValidationResult("Bitte eine gültige @htlvb.at E-Mail-Adresse eingeben.", new[] { nameof(Email) }));
                    }

                    // Eingeladene Mitglieder
                    foreach(string invitedMembers in invitedUsers)
                    {
                        if(invitedMembers.ToLower() == Email.ToLower())
                        {
                            errors.Add(new ValidationResult("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", new[] { nameof(Email) }));
                            return errors;
                        }
                    }

                    // Mitglieder in der Datenbank
                    var groupId = dbContext.Gruppen.Where(g => g.Event_EventID == selectedEventId && g.GruppenleiterId == usernameLoggedInUser).Select(g => g.GruppenID).FirstOrDefault();
                    var membersInDatabase = dbContext.UserInGruppe.Where(gr => gr.Gruppe_GruppenId == groupId).Select(us => us.User.Email);

                    foreach(var memberInDatabase in membersInDatabase)
                    {
                        if(memberInDatabase.ToLower() == Email.ToLower())
                        {
                            errors.Add(new ValidationResult("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", new[] { nameof(Email) }));
                            return errors;
                        }
                    }

                    // Prüfen, ob die Person bereits am Event teilnimmt
                    if (usernameNewMember != null)
                    {
                        var groupIds = dbContext.UserInGruppe
                            .Where(ug => ug.User_UserId.ToUpper() == usernameNewMember)
                            .Select(ug => ug.Gruppe_GruppenId)
                            .ToList();

                        if (groupIds.Any())
                        {
                            var eventIdExists = dbContext.Gruppen
                                .Any(g => groupIds.Contains(g.GruppenID) && g.Event_EventID == selectedEventId);

                            if (eventIdExists)
                            {
                                errors.Add(new ValidationResult("Diese Person nimmt bereits am Event teil. Um sie/ihn trotzdem ins Team zu holen, muss sie/er sich zuerst wieder abmelden.", new[] { nameof(Email) }));
                            }
                        }
                    }
                }

                return errors;
            }
        }
    }
}