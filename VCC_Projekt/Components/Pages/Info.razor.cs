namespace VCC_Projekt.Components.Pages
{
    public partial class Info
    {
        private bool isLoggedIn;

        protected override async Task OnInitializedAsync()
        {
            await InitializeAuthState();
        }

        private async Task InitializeAuthState()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isLoggedIn = authState.User.Identity.IsAuthenticated;
        }
    }
}
