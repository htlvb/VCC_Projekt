namespace VCC_Projekt.Components.Account
{
    public class MailOptions
    {
        public const string MailOptionsKey = "MailOptions";

        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
