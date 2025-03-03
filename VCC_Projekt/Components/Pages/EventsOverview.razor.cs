using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventsOverview
    {
        private List<Event> events;
        private string userId;
        private List<Gruppe> userGroups = new List<Gruppe>();

        private List<ValidationResult> addMemberErrors = new List<ValidationResult>();

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
                }
            }
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

        private async Task AddMember(int groupId, int eventId)
        {
            // ähnlich wie bei Event Registration
        }
    }
}
