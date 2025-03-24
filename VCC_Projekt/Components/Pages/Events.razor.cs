namespace VCC_Projekt.Components.Pages
{
    public partial class Events
    {
        private bool isLoggedIn;
        private string buttonLink;

        protected override async Task OnInitializedAsync()
        {
            await InitializeAuthState();
        }

        private async Task InitializeAuthState()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isLoggedIn = authState.User.Identity.IsAuthenticated;
        }

        private string GetEventLink(int eventId)
        {
            if (isLoggedIn)
            {
                return $"/signup-event?eventId={eventId}";
            }
            else
            {
                return "/Account/Register";
            }
        }
    }
}
