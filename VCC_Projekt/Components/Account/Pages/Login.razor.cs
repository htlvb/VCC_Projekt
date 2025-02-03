using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VCC_Projekt.Components.Account.Pages
{
    public partial class Login
    {
        private string? errorMessage;
        

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }

        public async Task LoginUser()
        {
            var user = await Usermanager.FindByEmailAsync(Input.EmailOrUsername) 
                ?? await Usermanager.FindByNameAsync(Input.EmailOrUsername);

            if (user == null)
            {
                errorMessage = "Error: Invalid login attempt.";
                return;
            }
            string userName = user.UserName ?? "";
            var result = await SignInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            switch (result)
            {
                case { Succeeded: true }:
                    Logger.LogInformation("User logged in.");
                    RedirectManager.RedirectTo(ReturnUrl);
                    break;

                case { RequiresTwoFactor: true }:
                    RedirectManager.RedirectTo(
                        "Account/LoginWith2fa",
                        new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
                    break;

                case { IsLockedOut: true }:
                    Logger.LogWarning("User account locked out.");
                    RedirectManager.RedirectTo("Account/Lockout");
                    break;

                default:
                    errorMessage = "Error: Invalid login attempt.";
                    break;
            }
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "E-Mail oder Benutzername ist erforderlich.")]
            public string EmailOrUsername { get; set; } = "";

            [Required(ErrorMessage = "Passwort ist erforderlich.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Angemeldet bleiben")]
            public bool RememberMe { get; set; }
        }
    }
}
