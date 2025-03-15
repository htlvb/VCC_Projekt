using Microsoft.Extensions.Diagnostics.HealthChecks;
using MimeKit;
using MudBlazor;
using System.Net.Mail;

namespace VCC_Projekt.Components.Pages
{
    public partial class SupportEmailPage
    {
        private bool isEmailDialogVisible = false;
        string curSubject;
        private string fixedEmail;
        private List<MimeMessage> supportEmails = new();
        private List<MimeMessage> unansweredEmails = new();
        private List<MimeMessage> answeredEmails = new();
        private string _searchString = string.Empty;
        private int activeTabIndex = 0;
        private MimeMessage baseMessage = null;
        private HashSet<MimeMessage> selectedEmails = new();

        protected override async Task OnInitializedAsync()
        {
            Snackbar.Add("Emails werden geladen. Bitte warten ...", Severity.Info);
            supportEmails = await EmailService.GetEmailsAsync("support");
            CategorizeEmails();
            Snackbar.Clear();
            Snackbar.Add("Emails erfolgreich geladen.",Severity.Success);
        }

        private void CategorizeEmails()
        {
            unansweredEmails = supportEmails.Where(email => !IsEmailAnswered(email)).ToList();
            answeredEmails = supportEmails.Where(IsEmailAnswered).ToList();
        }

        private bool IsEmailAnswered(MimeMessage email)
        {
            return email.From.Mailboxes.Any(m => m.Address.Contains("vcc",StringComparison.OrdinalIgnoreCase));
        }

        private void OpenEmailDialog(MimeMessage email)
        {
            fixedEmail = FormatEmail(email.From.Mailboxes.FirstOrDefault().Address);
            if(!email.Subject.StartsWith("AW",StringComparison.OrdinalIgnoreCase)) curSubject = $"AW: {email.Subject}";
            else curSubject = email.Subject;
            baseMessage = email;
            isEmailDialogVisible = true;
        }

        private string FormatEmail(string email)
        {
            // Überprüfen, ob die E-Mail mit @htlvb.at endet
            if (email.EndsWith("@htlvb.at", StringComparison.OrdinalIgnoreCase))
            {
                // Teile der E-Mail-Adresse extrahieren
                var parts = email.Split('@');
                var localPart = parts[0]; // Teil vor dem @
                var domain = parts[1];    // Teil nach dem @

                // Lokalen Teil in "m.maier" umwandeln
                var nameParts = localPart.Split('.');
                if (nameParts.Length == 2)
                {
                    var firstName = nameParts[0];
                    var lastName = nameParts[1];

                    // Formatierte E-Mail-Adresse erstellen
                    return $"{firstName[0]}.{lastName}@{domain}".ToLower();
                }
            }

            // Falls die E-Mail nicht mit @htlvb.at endet, unverändert zurückgeben
            return email;
        }

        private void ReadEmailBody(MimeMessage email)
        {
            var parameters = new DialogParameters();
            parameters.Add("EmailBody", email.HtmlBody ?? email.TextBody); // Oder email.HtmlBody für HTML-Inhalt
            // parameters.Add("Attachments", email.Attachments.Select(a => new Attachment(null, null)).ToList());

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

            DialogService.ShowAsync<EmailViewDialog>("E-Mail-Body", parameters, options);
        }

        private async Task DeleteEmail(MimeMessage email)
        {
            var parameters = new DialogParameters();
            parameters.Add("Question", "Willst du die Email wirklich löschen?");
            var result = await DialogService.ShowAsync<DeleteEmailDialog>("E-Mail löschen", parameters);
            StateHasChanged();
            var dialogResult = await result.Result;
            if (!dialogResult.Canceled)
            {
                Snackbar.Add("Emails werden gelöscht...", Severity.Info);
                await EmailService.DeleteEmailsAsync(new List<string>() { email.MessageId });
                supportEmails.Remove(email);
                CategorizeEmails(); // Kategorien aktualisieren
                Snackbar.Clear();
                Snackbar.Add("E-Mail wurde gelöscht.", Severity.Success);
            }
            StateHasChanged();
        }

        private async Task DeleteEmails()
        {
            var parameters = new DialogParameters();
            parameters.Add("Question", $"Willst du wirklich {selectedEmails.Count} Emails löschen?");
            var result = await DialogService.ShowAsync<DeleteEmailDialog>("E-Mail löschen", parameters);
            StateHasChanged();
            var dialogResult = await result.Result;
            if (!dialogResult.Canceled)
            {
                Snackbar.Add("Emails werden gelöscht...", Severity.Info);
                await EmailService.DeleteEmailsAsync(new List<string>(selectedEmails.Select(e => e.MessageId).ToList()));
                supportEmails.RemoveAll(e => selectedEmails.Contains(e));
                selectedEmails.Clear();
                CategorizeEmails(); // Kategorien aktualisieren
                Snackbar.Clear();
                Snackbar.Add("E-Mail wurde gelöscht.", Severity.Success);
            }
            StateHasChanged();
        }

        private Func<MimeMessage, bool> _quickFilter => x =>
        {
            if (string.IsNullOrEmpty(x.Subject)) return true;
            return true;
        };

        private void ClearSelectedEmails()
        {
            selectedEmails.Clear();
        }


    }
}
