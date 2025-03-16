using MailKit;
using MimeKit;
using MudBlazor;
using System.Net.Mail;

namespace VCC_Projekt.Components.Pages
{
    public partial class SupportEmailPage
    {
        private List<MimeMessage> supportEmails = new();
        private List<MimeMessage> unansweredEmails = new();
        private List<MimeMessage> answeredEmails = new();
        private string _searchString = string.Empty;
        private int activeTabIndex = 0;
        private HashSet<MimeMessage> selectedEmails = new();

        protected override async Task OnInitializedAsync()
        {
            Snackbar.Add("Emails werden geladen. Bitte warten ...", Severity.Info);
            supportEmails = await EmailService.GetEmailsAsync("support");
            CategorizeEmails();
            Snackbar.Clear();
            Snackbar.Add("Emails erfolgreich geladen.", Severity.Success);
        }

        private void CategorizeEmails()
        {
            unansweredEmails = supportEmails.Where(email => !IsEmailAnswered(email)).ToList();
            answeredEmails = supportEmails.Where(IsEmailAnswered).ToList();
        }

        private bool IsEmailAnswered(MimeMessage email)
        {
            // Ihre E-Mail-Adresse (z. B. "ihre-email@example.com")
            string yourEmail = EmailService.emailAddress;

            // Überprüfen, ob die E-Mail von Ihnen stammt (d. h. Sie haben geantwortet)
            bool isFromYou = email.From.Mailboxes.Any(m => m.Address.Equals(yourEmail, StringComparison.OrdinalIgnoreCase));

            // Überprüfen, ob die E-Mail als beantwortet markiert ist
            // bool isAnswered = email. .HasFlag(MessageFlags.Answered);

            // Wenn die E-Mail von Ihnen stammt, eine Antwort ist oder bereits als beantwortet markiert ist, gilt sie als beantwortet
            return isFromYou; //isAnswered;
        }

        private async Task OpenEmailDialog(MimeMessage email)
        {
            string curSubject = "";
            if (!email.Subject.StartsWith("AW", StringComparison.OrdinalIgnoreCase)) curSubject = $"AW: {email.Subject}";
            else curSubject = email.Subject;

            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters
            {
                { "UseDropdown", false },
                { "FixedEmail", email.From.Mailboxes.FirstOrDefault().Address },
                { "ReadOnly", true },
                { "Subject", curSubject },
                { "baseMessageMail", email }
            };

            var dialog = await DialogService.ShowAsync<EmailSendDialog>("Support Email Senden", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                EmailService.MarkEmailAsAnswered(email.MessageId);
                unansweredEmails.Remove(email);
                StateHasChanged();
            }

        }

        private void ReadEmailBody(MimeMessage email)
        {
            var parameters = new DialogParameters();
            parameters.Add("EmailBody", email.HtmlBody ?? email.TextBody); // Oder email.HtmlBody für HTML-Inhalt
            parameters.Add("Attachments", email.Attachments.OfType<MimePart>().Select(a => new Attachment(a.Content.Open(), a.FileName)).ToList());

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
            selectedEmails = selectedEmails;
            parameters.Add("Question", $"Willst du wirklich {selectedEmails.Count} Emails löschen?");
            var result = await DialogService.ShowAsync<DeleteEmailDialog>("E-Mail löschen", parameters);
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
            if (string.IsNullOrEmpty(_searchString)) return true;
            if (x.From.Mailboxes.FirstOrDefault().Address.Contains(_searchString, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.Subject.Contains(_searchString, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.Date.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase)) return true;
            if (x.From.Mailboxes.FirstOrDefault().Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        };

        private Task UpdateSelection(HashSet<MimeMessage> messages)
        {
            selectedEmails = messages;
            return Task.CompletedTask;
        }

        private void ClearSelectedEmails()
        {
            selectedEmails.Clear();
        }
        private async Task RefreshEmails()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }


    }
}
