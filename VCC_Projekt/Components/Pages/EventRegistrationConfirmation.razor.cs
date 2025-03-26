using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventRegistrationConfirmation
    {
        private string teamname = null;
        private int eventId;

        protected override void OnInitialized()
        {
            var uri = new Uri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("teamname", out var teamnameValue))
            {
                teamname = teamnameValue;
            }

            if (queryParams.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue, out int parsedEventId))
            {
                eventId = parsedEventId;
            }
        }
    }
}
