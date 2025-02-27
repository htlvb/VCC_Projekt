using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace VCC_Projekt.Components.Pages
{
    public partial class MyEvents
    {
        private List<Event> events;

        protected override async Task OnInitializedAsync()
        {
            var user = await UserManager.GetUser Async((await AuthenticationStateProvider.GetAuthenticationStateAsync()).User);
            events = await dbContext.Gruppe
                .Where(g => g.UserInGruppe.Any(u => u.User_UserId == user.UserName)) 
                .SelectMany(g => g.Events) 
                .ToListAsync();
        }

        private async Task Unregister(int eventId)
        {
            var user = await UserManager.GetUser Async((await AuthenticationStateProvider.GetAuthenticationStateAsync()).User);
            var eventToRemove = await dbContext.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == user.Id);

            if (eventToRemove != null)
            {
                dbContext.EventParticipants.Remove(eventToRemove);
                await dbContext.SaveChangesAsync();
                events = await dbContext.Events
                    .Where(e => e.Participants.Any(p => p.UserId == user.Id))
                    .ToListAsync();
            }
        }
    }
}
