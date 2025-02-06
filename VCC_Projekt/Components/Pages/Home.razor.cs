namespace VCC_Projekt.Components.Pages
{
    public partial class Home
    {
        private bool isLoggedIn;
        private string buttonLink;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isLoggedIn = authState.User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                // Wenn der Benutzer angemeldet ist, weiterleiten
                buttonLink = "/signup-event"; // Zielseite für angemeldete Benutzer
            }
            else
            {
                // Wenn der Benutzer nicht angemeldet ist, setzen Sie den Link für die Anmeldung
                buttonLink = "/Account/Register"; // Link zur Registrierungsseite
            }
        }
    }
}
