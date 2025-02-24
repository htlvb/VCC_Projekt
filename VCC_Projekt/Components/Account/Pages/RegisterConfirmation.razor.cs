using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.RegularExpressions;

namespace VCC_Projekt.Components.Account.Pages
{
    public partial class RegisterConfirmation
    {
        private string? statusMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string teamname;
        private string eventId;

        protected override async Task OnInitializedAsync()
        {
            if (Email is null)
            {
                RedirectManager.RedirectTo("");
            }

            var user = await UserManager.FindByEmailAsync(Email);
            if (user is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                statusMessage = "Error finding user for unspecified email";
            }

            var uri = new Uri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("teamname", out var eventIdValue))
            {
                eventId = eventIdValue;
            }

            if (queryParams.TryGetValue("eventId", out var eventIdValue) && int.TryParse(eventIdValue, out int parsedEventId))
            {
                groupId = parsedEventId;
            }
        }
    }
}
