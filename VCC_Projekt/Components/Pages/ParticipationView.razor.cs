using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace VCC_Projekt.Components.Pages
{
    public partial class ParticipationView
    {
        [Parameter]
        public int EventId { get; set; }

        private Event? Event { get; set; } = null;
        private Level? CurrentLevel;
        private Dictionary<int, byte[]> UploadedFiles { get; set; } = new();
        private ClaimsPrincipal User { get; set; }
        private Gruppe? Group { get; set; }
        private bool AllFilesSubmitted { get; set; } = false;
        private bool isLoading = false;
        private bool accessDenied = false;
        private string accessDeniedMessage = "";

        protected override void OnInitialized()
        {
            isLoading = true;
            if (EventId <= 0 || !dbContext.Events.Any(ev => ev.EventID == EventId))
            {
                accessDenied = true;
                accessDeniedMessage = "Event existiert nicht";
                return;
            }

            var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;


            User = authState.User;

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                accessDenied = true;
                accessDeniedMessage = "Benutzer nicht gefunden";
                return;
            }

            Group = dbContext.Gruppen
                .Where(gr => gr.Event_EventID == EventId && gr.UserInGruppe.Any(us => us.User_UserId == User.Identity.Name))
                .Include(g => g.Absolviert)
                .Include(n => n.UserInGruppe)
                .FirstOrDefault();

            if (Group == null)
            {
                accessDenied = true;
                accessDeniedMessage = "Gruppe nicht gefunden";
                return;
            }
            if (Group.Gesperrt)
            {
                accessDenied = true;
                accessDeniedMessage = "Gruppe gesperrt";
                return;
            }

            Event = dbContext.Events
                            .Where(e => e.EventID == EventId)
                            .Include(e => e.Levels)
                            .FirstOrDefault();
            CurrentLevel = dbContext.Levels
                                    .Where(level => level.Event_EventID == EventId)
                                    .Where(level => !dbContext.GruppeAbsolviertLevels
                                        .Any(a => a.Level_LevelID == level.LevelID && a.Gruppe_GruppeID == Group.GruppenID))
                                    .Include(l => l.Aufgaben)
                                    .FirstOrDefault();

            if (Event == null)
            {
                accessDenied = true;
                accessDeniedMessage = "Event nicht gefunden";
                return;
            }

            isLoading = false;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (accessDenied) return;
                await JS.InvokeVoidAsync("startTimer");
            }
        }

        private async Task UploadFile(IBrowserFile file, int aufgabenId)
        {
            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            UploadedFiles[aufgabenId] = memoryStream.ToArray();
        }

        private async Task SubmitFile(Aufgabe aufgabe)
        {
            if (UploadedFiles.TryGetValue(aufgabe.AufgabenID, out var uploadedFile))
            {
                if (uploadedFile.SequenceEqual(aufgabe.Ergebnis_TXT))
                {
                    //TODO
                }
                else
                {
                    return;
                }
            }

            // Überprüfen, ob alle Dateien korrekt eingereicht wurden
            AllFilesSubmitted = CurrentLevel?.Aufgaben.All(a =>
                UploadedFiles.ContainsKey(a.AufgabenID) &&
                UploadedFiles[a.AufgabenID].SequenceEqual(a.Ergebnis_TXT)) ?? false;
        }

        private async Task ProceedToNextLevel()
        {
            var absolviert = new GruppeAbsolviertLevel
            {
                Gruppe_GruppeID = Group.GruppenID,
                Level_LevelID = CurrentLevel.LevelID,
                Fehlversuche = 0
            };

            dbContext.GruppeAbsolviertLevels.Add(absolviert);
            await dbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/participation/{EventId}");
        }
    }
}