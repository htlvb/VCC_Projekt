using Microsoft.AspNetCore.Components;

namespace VCC_Projekt.Components.Pages
{
    public partial class Events
    {
        private bool isLoggedIn;
        private string buttonLink;

        private async Task<string> GetEventLink(int eventId)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isLoggedIn = authState.User.Identity.IsAuthenticated;


            if (isLoggedIn)
            {
                // Wenn der Benutzer eingeloggt ist, leite ihn zur Event-Anmeldeseite mit der Event-ID
                return $"/signup-event/{eventId}";
            }
            else
            {
                // Wenn der Benutzer nicht eingeloggt ist, leite ihn zur Login- oder Registrierungsseite
                return "/Account/Register"; // Du kannst hier auch "/Account/Login" verwenden
            }
        }
    }
}
