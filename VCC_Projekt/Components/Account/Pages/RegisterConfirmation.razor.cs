using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace VCC_Projekt.Components.Account.Pages
{
    public partial class RegisterConfirmation
    {
        private string? emailConfirmationLink;
        private string? statusMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

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
        }
    }
}
