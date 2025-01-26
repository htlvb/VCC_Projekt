using MailKit.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using VCC_Projekt.Data;
using System.Net;
using System.Net.Mail;

namespace VCC_Projekt.Components.Account.Pages
{
    public partial class Register
    {
        private IEnumerable<IdentityError>? identityErrors;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        public async Task RegisterUser(EditContext editContext)
        {
            var user = CreateUser();
            user.Firstname = Input.Firstname;
            user.Lastname = Input.Lastname;
            await UserStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var result = await UserManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                identityErrors = result.Errors;
                return;
            }

            Logger.LogInformation("User created a new account with password.");

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));
            Logger.LogInformation("Email sent");

            if (UserManager.Options.SignIn.RequireConfirmedAccount)
            {
                RedirectManager.RedirectTo(
                    "Account/RegisterConfirmation",
                    new() { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
            }

            await SignInManager.SignInAsync(user, isPersistent: false);
            RedirectManager.RedirectTo(ReturnUrl);
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)UserStore;
        }

        public interface IEmailSender
        {
            Task<bool> SendConfirmationLinkAsync(ApplicationUser user, string email, string callbackUrl);
        }

        public class EmailSenderService : IEmailSender
        {
            public async Task<bool> SendConfirmationLinkAsync(ApplicationUser user, string email, string callbackUrl)
            {
                var message = new MailMessage();
                message.From = new MailAddress("vcc.htlvb@gmail.com");
                message.To.Add(email); // Empfänger-E-Mail-Adresse
                message.Subject = "Bestätige deine E-Mail-Adresse";
                message.Body = $"Bitte klicke auf den folgenden Link, um deine E-Mail-Adresse zu bestätigen: <a href='{callbackUrl}'>Bestätige deine E-Mail-Adresse</a>";
                message.IsBodyHtml = true; // HTML-Inhalt

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("vcc.htlvb@gmail.com", "!Passw0rd");
                    smtpClient.EnableSsl = true; // SSL aktivieren

                    try
                    {
                        await smtpClient.SendMailAsync(message);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Hier kannst du den Fehler protokollieren oder eine Ausnahme werfen
                        Console.WriteLine($"Fehler beim Senden der E-Mail: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "E-Mail ist erforderlich.")]
            [EmailAddress]
            [RegularExpression(@"^[a-zA-Z0-9._-]+@[Hh][Tt][Ll][Vv][Bb]\.[Aa][Tt]$",
                ErrorMessage = "Bitte geben Sie eine gültige @htlvb.at " +
                "E-Mail-Adresse ein. Erlaubte Sonderzeichen sind ( . - _ )")]
            [Display(Name = "E-Mail")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Vorname ist erforderlich.")]
            [DataType(DataType.Text)]
            [Display(Name = "Vorname")]
            public string Firstname { get; set; } = "";

            [Required(ErrorMessage = "Nachname ist erforderlich.")]
            [DataType(DataType.Text)]
            [Display(Name = "Nachname")]
            public string Lastname { get; set; } = "";

            [Required(ErrorMessage = "Benutzername ist erforderlich.")]
            [StringLength(100, ErrorMessage = "Der Benutzername muss mind. {2} Zeichen lang sein.", MinimumLength = 3)]
            [RegularExpression(@"^(?!.*@[Hh][Tt][Ll][Vv][Bb]\.[Aa][Tt]).*$",
                ErrorMessage = "Die Schul-Domain '@htlvb.at' ist im Benutzernamen nicht erlaubt.")]
            [DataType(DataType.Text)]
            [Display(Name = "Benutzername")]
            public string Username { get; set; } = "";

            [Required(ErrorMessage = "Passwort ist erforderlich.")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
                ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein und Groß-/Kleinbuchstaben, Zahlen sowie Sonderzeichen enthalten.")]
            [Display(Name = "Passwort")]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Bestätigungspasswort ist erforderlich.")]
            [DataType(DataType.Password)]
            [Display(Name = "Passwort bestätigen")]
            [Compare("Password", ErrorMessage = "Die Passwörter stimmen nicht überein.")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
