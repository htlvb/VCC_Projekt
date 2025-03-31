using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VCC_Projekt.Components.Account
{
    public class MailOptions
    {
        public const string MailOptionsKey = "MailOptions";

        public string Email { get; set; }
        public string Password { get; set; }
        public string SmptServer { get; set; }
        public string ImapServer { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class AccessTokenModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
    }
}
