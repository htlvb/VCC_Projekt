namespace VCC_Projekt.Components.Pages
{
    public partial class ParticipationView
    {
        private string teamName;

        protected override async Task OnInitializedAsync()
        {
            teamName = HttpContextAccessor.HttpContext.Session.GetString("TeamName");
        }






    }
}
