using MimeKit;
using MudBlazor;

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
            return email.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase);
        }

        private void OpenEmailDialog(MimeMessage email)
        {
            fixedEmail = email.From.Mailboxes.FirstOrDefault().Address;
            curSubject = $"Re: {email.Subject}";
            isEmailDialogVisible = true;
        }

        private void ReadEmailBody(MimeMessage email)
        {
            // Implement the logic to read the email body
        }

        private void DeleteEmail(MimeMessage email)
        {
            // Implement the logic to delete the email from the mailbox
        }

        private Func<MimeMessage, bool> _quickFilter => x =>
        {
            if (string.IsNullOrEmpty(x.Subject)) return true;
            return true;
        };


    }
}
