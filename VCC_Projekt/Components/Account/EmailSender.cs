using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Text;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly MailOptions _options;
    private ImapClient _imapClient;
    private SmtpClient _smtpClient;
    private AccessTokenModel _currentToken;
    private DateTime _tokenExpiryTime;
    private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);
    public string EmailAddress => _options.Email;

    public EmailSender(IOptions<MailOptions> mailOptions)
    {
        _options = mailOptions.Value;
    }

    private async Task EnsureImapConnectedAsync()
    {
        if (_imapClient != null && _imapClient.IsConnected) return;

        try
        {
            var token = await GetOrRefreshTokenAsync();

            if (_imapClient != null)
            {
                await _imapClient.DisconnectAsync(true);
                _imapClient.Dispose();
            }

            _imapClient = new ImapClient();
            await _imapClient.ConnectAsync(_options.ImapServer, 993, SecureSocketOptions.SslOnConnect)
                            .ConfigureAwait(false);

            var oauth2 = new SaslMechanismOAuth2(_options.Email, token.access_token);
            await _imapClient.AuthenticateAsync(oauth2).ConfigureAwait(false);
        }
        catch (AuthenticationException)
        {
            // Bei Auth-Fehlern mit frischem Token versuchen
            var freshToken = await GetOrRefreshTokenAsync(forceRefresh: true);
            var oauth2 = new SaslMechanismOAuth2(_options.Email, freshToken.access_token);
            await _imapClient.AuthenticateAsync(oauth2).ConfigureAwait(false);
        }
        catch (ImapCommandException ex) when (ex.ResponseText?.Contains("AUTHENTICATE failed") == true)
        {
            // Spezifische Behandlung von IMAP Auth-Fehlern
            var freshToken = await GetOrRefreshTokenAsync(forceRefresh: true);
            await _imapClient.DisconnectAsync(true);
            await _imapClient.ConnectAsync(_options.ImapServer, 993, SecureSocketOptions.SslOnConnect);
            await _imapClient.AuthenticateAsync(new SaslMechanismOAuth2(_options.Email, freshToken.access_token));
        }
    }

    private async Task<AccessTokenModel> GetOrRefreshTokenAsync(bool forceRefresh = false)
    {
        // Wenn Token noch gültig (mit 2 Minuten Puffer)
        if (!forceRefresh && _currentToken != null && DateTime.UtcNow < _tokenExpiryTime)
        {
            return _currentToken;
        }

        await _tokenLock.WaitAsync();
        try
        {
            // Double-Check nach Lock
            if (!forceRefresh && _currentToken != null && DateTime.UtcNow < _tokenExpiryTime)
            {
                return _currentToken;
            }

            var newToken = await GetAccessTokenAsync();
            _currentToken = newToken;

            // Verwende den kürzeren der beiden Expiry-Werte mit Puffer
            var expiresIn = Math.Min(newToken.expires_in, newToken.ext_expires_in) - 120;
            _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresIn);

            return _currentToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public async Task<AccessTokenModel> GetAccessTokenAsync()
    {
        string url = $"https://login.microsoftonline.com/{_options.TenantId}/oauth2/v2.0/token";

        var data = new Dictionary<string, string>
        {
            {"grant_type", "client_credentials"},
            {"scope", "https://outlook.office365.com/.default"},
            {"client_id",  _options.ClientId},
            {"client_secret", _options.ClientSecret}
        };

        using HttpClient client = new();
        var response = await client.PostAsync(url, new FormUrlEncodedContent(data));
        return await response.Content.ReadFromJsonAsync<AccessTokenModel>();
    }

    private void EnsureSmtpConnected()
    {
        if (_smtpClient == null)
        {
            _smtpClient = new SmtpClient(_options.SmptServer, 587)
            {
                Credentials = new NetworkCredential(_options.Email, _options.Password),
                EnableSsl = true
            };
        }
    }

    public async Task SendEmailAsync(MailMessage message)
    {
        EnsureSmtpConnected();
        message.From = new MailAddress(_options.Email);
        message.BodyEncoding = Encoding.UTF8;

        try
        {
            await _smtpClient.SendMailAsync(message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending Email: {ex.Message} To: {string.Join(",", message.To)}");
        }
    }

    public async Task SendBulkEmailsAsync(List<string> recipients, string subject, string body, List<Attachment> attachments)
    {
        EnsureSmtpConnected();
        var tasks = recipients.Select(email =>
        {
            var message = new MailMessage(_options.Email, email, subject, body)
            {
                IsBodyHtml = true
            };
            foreach (var attachment in attachments)
            {
                message.Attachments.Add(attachment);
            }
            return SendEmailAsync(message);
        }).ToList();
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task<List<(MimeMessage Message, MessageFlags? Flags)>> GetEmailsAsync(string filter = "")
    {
        await EnsureImapConnectedAsync().ConfigureAwait(false);
        var messagesWithFlags = new List<(MimeMessage Message, MessageFlags? Flags)>();

        try
        {
            // Posteingang durchsuchen
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly).ConfigureAwait(false);
            await SearchAndAddMessagesAsync(inbox, filter, messagesWithFlags).ConfigureAwait(false);

            // Gesendete E-Mails durchsuchen
            var sentFolder = _imapClient.GetFolder("Gesendete Elemente");

            if (sentFolder != null)
            {
                await sentFolder.OpenAsync(FolderAccess.ReadOnly).ConfigureAwait(false);
                await SearchAndAddMessagesAsync(sentFolder, filter, messagesWithFlags).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("Gesendete Elemente nicht gefunden!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abrufen der E-Mails: {ex.Message}");
        }

        return messagesWithFlags.OrderBy(e => e.Message.Date).ToList();
    }
    private async Task SearchAndAddMessagesAsync(IMailFolder folder, string filter, List<(MimeMessage Message, MessageFlags? Flags)> messagesWithFlags)
    {
        IList<UniqueId> uids;
        if (string.IsNullOrEmpty(filter))
        {
            uids = await folder.SearchAsync(SearchQuery.All).ConfigureAwait(false);
        }
        else
        {
            var query = SearchQuery.SubjectContains(filter).Or(SearchQuery.BodyContains(filter));
            uids = await folder.SearchAsync(query).ConfigureAwait(false);
        }

        var summaries = await folder.FetchAsync(uids, MessageSummaryItems.Flags | MessageSummaryItems.UniqueId).ConfigureAwait(false);

        foreach (var summary in summaries)
        {
            var message = await folder.GetMessageAsync(summary.UniqueId).ConfigureAwait(false);
            messagesWithFlags.Add((message, summary.Flags));
        }
    }

    public async Task DeleteEmailsAsync(List<string> messageIds)
    {
        await EnsureImapConnectedAsync().ConfigureAwait(false);

        var inboxFolder = _imapClient.Inbox;

        await DeleteEmailsInFolderAsync(inboxFolder, messageIds).ConfigureAwait(false);

        var sentFolder = _imapClient.GetFolder("Gesendete Elemente");

        await DeleteEmailsInFolderAsync(sentFolder, messageIds).ConfigureAwait(false);

    }

    private async Task DeleteEmailsInFolderAsync(IMailFolder folder, List<string> messageIds)
    {
        await folder.OpenAsync(FolderAccess.ReadWrite).ConfigureAwait(false);

        foreach (var messageId in messageIds)
        {
            var uids = await folder.SearchAsync(SearchQuery.HeaderContains("Message-Id", messageId)).ConfigureAwait(false);
            await folder.AddFlagsAsync(uids, MessageFlags.Deleted, true).ConfigureAwait(false);
        }

        await folder.ExpungeAsync().ConfigureAwait(false);
    }

    public async Task MarkEmailAsAnsweredByMessageIdAsync(string messageId)
    {
        await EnsureImapConnectedAsync().ConfigureAwait(false);

        var inboxFolder = _imapClient.Inbox;
        await inboxFolder.OpenAsync(FolderAccess.ReadWrite).ConfigureAwait(false);

        var emailSummaries = await inboxFolder.FetchAsync(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope).ConfigureAwait(false);

        UniqueId? emailId = null;
        foreach (var summary in emailSummaries)
        {
            if (summary.Envelope.MessageId == messageId)
            {
                emailId = summary.UniqueId;
                break;
            }
        }

        if (emailId.HasValue)
        {
            await inboxFolder.AddFlagsAsync(emailId.Value, MessageFlags.Answered, true).ConfigureAwait(false);
        }
        else
        {
            throw new Exception("Keine E-Mail mit der angegebenen MessageId gefunden.");
        }

    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        MailMessage message = new()
        {
            From = new MailAddress(_options.Email),
            Subject = "Bestätige deine E-Mail-Adresse",
            Body = @"
            <html>
            <head>
            <meta charset='utf-8' />
            <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 20px;
            }
            .container {
                max-width: 600px;
                margin: auto;
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333;
            }
            p {
                font-size: 16px;
                line-height: 1.5;
                color: #555;
            }
            a.button {
                display: inline-block;
                padding: 10px 20px;
                margin: 20px 0;
                background-color: #007BFF;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }
            a.button:hover {
                background-color: #0056b3;
            }
            </style>
            </head>
            <body>
            <div class='container'>
            <h1>Bitte bestätige deine E-Mail-Adresse!</h1>
            <p>Um deine E-Mail-Adresse zu bestätigen, klicke bitte auf den folgenden Button:</p>
            <a href='" + confirmationLink + @"' class='button'>E-Mail-Adresse bestätigen</a>
            <p>Wenn du diese Anfrage nicht gestellt hast, kannst du diese E-Mail ignorieren.</p>
            <p>Mit freundlichen Grüßen,<br>Dein VCC-Team</p>
            </div>
            </body>
            </html>",
            IsBodyHtml = true,
        };
        message.To.Add(email);
        await SendEmailAsync(message).ConfigureAwait(false);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        MailMessage message = new()
        {
            From = new MailAddress(_options.Email),
            Subject = "Setze dein Passwort zurück",
            Body = @"
            <html>
            <head>
            <meta charset='utf-8' />
            <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 20px;
            }
            .container {
                max-width: 600px;
                margin: auto;
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333;
            }
            p {
                font-size: 16px;
                line-height: 1.5;
                color: #555;
            }
            a.button {
                display: inline-block;
                padding: 10px 20px;
                margin: 20px 0;
                background-color: #007BFF;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }
            a.button:hover {
                background-color: #0056b3;
            }
            </style>
            </head>
            <body>
            <div class='container'>
            <h1>Passwort zurücksetzen!</h1>
            <p>Um dein Passwort zurückzusetzen, klicke bitte auf den folgenden Button:</p>
            <a href='" + resetLink + @"' class='button'>Passwort zurücksetzen</a>
            <p>Wenn du diese Anfrage nicht gestellt hast, kannst du diese E-Mail ignorieren.</p>
            <p>Mit freundlichen Grüßen,<br>Dein VCC-Team</p>
            </div>
            </body>
            </html>",
            IsBodyHtml = true,
        };
        message.To.Add(email);
        await SendEmailAsync(message).ConfigureAwait(false);
    }

    public async Task SendInvitationLinkAsync(string groupManagerUsername, string groupManagerEmail, string email, string teamName, string invitationLink, string registerLink)
    {
        MailMessage message = new()
        {
            From = new MailAddress(_options.Email),
            Subject = "Gruppeneinladung",
            Body = @"
            <html>
            <head>
            <meta charset='utf-8' />
            <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 20px;
            }
            .container {
                max-width: 600px;
                margin: auto;
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }
            h1 {
                color: #333;
            }
            p {
                font-size: 16px;
                line-height: 1.5;
                color: #555;
            }
            a.button {
                display: inline-block;
                padding: 10px 20px;
                margin: 20px 0;
                background-color: #007BFF;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }
            a.button:hover {
                background-color: #0056b3;
            }
            </style>
            </head>
            <body>
            <div class='container'>
            <h1>Gruppeneinladung!</h1>
            <p>Du wurdest von <strong>" + groupManagerUsername + @"</strong> (" + groupManagerEmail + @") eingeladen, um der Gruppe <strong>" + teamName + @"</strong> beizutreten.</p>" + (string.IsNullOrEmpty(registerLink) ? string.Empty : @"
            <p>Da du noch nicht registriert bist, klicke bitte auf den folgenden Button, um dich zu registrieren (erst wenn du registriert bist, kannst du einer Gruppe betreten):</p>
            <a href='" + registerLink + @"' class='button'>Registrieren</a>") + @"
            <p>Um der Gruppe beizutreten, klicke bitte auf den folgenden Button:</p>
            <a href='" + invitationLink + @"' class='button'>Gruppe beitreten</a>
            <p>Wenn du dieser Gruppe nicht betreten willst, kannst du diese E-Mail ignorieren.</p>
            <p>Mit freundlichen Grüßen,<br>Dein VCC-Team</p>
            </div>
            </body>
            </html>",
            IsBodyHtml = true,
        };
        message.To.Add(email);
        await SendEmailAsync(message).ConfigureAwait(false);
    }
}