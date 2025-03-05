using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Text.RegularExpressions;
using VCC_Projekt.Data;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventsOverview
    {
        private List<Event> events;
        private List<Event> filteredEvents = new List<Event>();
        private List<Gruppe> userGroups = new List<Gruppe>();
        private static string userId;

        private List<ValidationResult> addMemberErrors = new List<ValidationResult>();

        private bool isPastEventsActive = false;
        private bool isUpcomingEventsActive = false;

        private bool isAddingMember;

        public static int eventId;

        private MemberModel newMember = new MemberModel();

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.Identity.IsAuthenticated)
            {
                userId = user.Identity.Name;

                if (!string.IsNullOrEmpty(userId))
                {
                    userGroups = await dbContext.Gruppen
                        .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
                        .Include(g => g.UserInGruppe)
                        .ToListAsync();

                    events = await dbContext.Gruppen
                        .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
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
                userId = user.Identity.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Fehler: Benutzer-ID konnte nicht ermittelt werden.");
                    return;
                }

                var userGroupEntry = await dbContext.UserInGruppe
                    .FirstOrDefaultAsync(uig => uig.User_UserId == userId &&
                                                dbContext.Gruppen.Any(g => g.GruppenID == uig.Gruppe_GruppenId && g.Event_EventID == eventId));

                if (userGroupEntry == null)
                {
                    Console.WriteLine($"Benutzer {userId} ist für Event {eventId} in keiner Gruppe.");
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
                    .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
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
                    .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
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
                var groupManagerEmail = dbContext.Users.Where(u => u.UserName == userId).Select(u => u.Email).FirstOrDefault();

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

                await EmailSender.SendInvitationLinkAsync(userId, groupManagerEmail, newMemberEmail, teamName, HtmlEncoder.Default.Encode(invaitationLink), HtmlEncoder.Default.Encode(registerLink));

                NavigationManager.NavigateTo($"/signup-event-confirmation?teamname={teamName}&eventId={eventId}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error in Registration: {ex.Message}");
            }

            isAddingMember = false; // Schließen Sie das Eingabefeld nach dem Hinzufügen
            newMemberEmail = string.Empty; // Eingabefeld zurücksetzen
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
                    var username = dbContext.Users.Where(u => u.NormalizedEmail == Email.Normalize()).Select(u => u.NormalizedUserName).FirstOrDefault();

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
                                errors.Add(new ValidationResult("Diese Person nimmt bereits am Event teil. Um sie/ihn trotzdem ins Team zu holen, muss sie/er sich zuerst wieder abmelden.", new[] { nameof(Email) }));
                            }
                        }
                    }

                    if (Email == dbContext.Users.Where(u => u.UserName == userId).Select(u => u.Email).First())
                    {
                        errors.Add(new ValidationResult("Du bist bereits Mitglieder der Gruppe.", new[] { nameof(newMemberEmail) }));
                    }

                    else if (!Regex.IsMatch(Email, @"(?i)^.+@htlvb\.at$"))
                    {
                        errors.Add(new ValidationResult("Bitte eine gültige @htlvb.at E-Mail-Adresse eingeben.", new[] { nameof(newMemberEmail) }));
                    }

                    //else if (Input.TeamMembers.Contains(Email))
                    //{
                    //    errors.Add(new ValidationResult("Diese E-Mail-Adresse ist bereits der Gruppe hinzugefügt.", new[] { nameof(newMemberEmail) }));
                    //}

                    //else if (Input.TeamMembers.Count >= 4)
                    //{
                    //    errors.Add(new ValidationResult("Die maximale Gruppengröße von 4 Teilnehmern wurde bereits erreicht.", new[] { nameof(newMemberEmail) }));
                    //}
                }

                return errors;
            }
        }
    }
}