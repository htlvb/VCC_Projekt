using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;

namespace VCC_Projekt.Components.Account.Pages
{
    public partial class ResendEmailConfirmation
    {
        private string? message;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        private async Task OnValidSubmitAsync()
        {
            var user = await UserManager.FindByEmailAsync(Input.Email!);
            if (user is null)
            {
                message = "Bestätigungsmail erfolgreich gesendet. Bitte überprüfe deine E-Mails.";
                return;
            }

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            message = "Bestätigungsmail erfolgreich gesendet. Bitte überprüfe deine E-Mails.";
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "E-Mail ist erforderlich.")]
            [EmailAddress]
            public string Email { get; set; } = "";
        }
    }
}
