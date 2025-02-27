namespace VCC_Projekt.Components.Pages
{
    public partial class MyEvents
    {
        private List<Event> events;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.Identity.IsAuthenticated)
            {
                var userId = user.Identity.Name; // Hier wird der Benutzername verwendet
                events = await dbContext.Gruppen
                    .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
                    .SelectMany(g => g.Event)
                    .ToListAsync();
            }
        }

        private async Task Unregister(int eventId)
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.Identity.IsAuthenticated)
            {
                var userId = user.Identity.Name; // Hier wird der Benutzername verwendet
                var eventToRemove = await dbContext.EventParticipants
                    .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);

                if (eventToRemove != null)
                {
                    dbContext.EventParticipants.Remove(eventToRemove);
                    await dbContext.SaveChangesAsync();
                    // Aktualisiere die Liste der Events nach der Abmeldung
                    events = await dbContext.Gruppe
                        .Where(g => g.UserInGruppe.Any(u => u.User_UserId == userId))
                        .SelectMany(g => g.Events)
                        .ToListAsync();
                }
            }
        }
    }
}
