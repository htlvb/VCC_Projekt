using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.IO.Compression;
using System.Security.Claims;

namespace VCC_Projekt.Components.Pages
{
    public partial class ParticipationView
    {
        [Parameter]
        public int EventId { get; set; }

        private Event? Event { get; set; } = null;
        private Level? CurrentLevel;
        private Dictionary<int, UploadedFile> UploadedFiles { get; set; } = new();
        private ClaimsPrincipal User { get; set; }
        private Gruppe? Group { get; set; }
        private bool AllFilesSubmitted { get; set; } = false;
        private bool isLoading = false;
        private bool accessDenied = false;
        private string accessDeniedMessage = "";
        private int Fehlversuche;

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
                             .Select(e => new Event
                             {
                                 EventID = e.EventID,
                                 Bezeichnung = e.Bezeichnung,
                                 Beginn = e.Beginn,
                                 Dauer = e.Dauer,
                                 StrafminutenProFehlversuch = e.StrafminutenProFehlversuch,
                                 Levels = e.Levels.Select(l => new Level
                                 {
                                     LevelID = l.LevelID,
                                     Levelnr = l.Levelnr
                                 }).ToList()
                             })
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


            DateTime now = DateTime.Now;
            if (!(now >= Event.Beginn && now <= Event.Beginn.AddMinutes(Event.Dauer)))
            {
                accessDenied = true;
                accessDeniedMessage = "Event wird aktuell nicht ausgeführt";
                return;
            }
            if (CurrentLevel.Aufgaben.Count == 0) AllFilesSubmitted = true;


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


        private async Task<string> GenerateZip()
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var aufgabe in CurrentLevel?.Aufgaben ?? new List<Aufgabe>())
                {
                    if (aufgabe.Input_TXT != null && aufgabe.Input_TXT.Length > 0)
                    {
                        var entry = archive.CreateEntry($"Level{CurrentLevel.Levelnr}_{aufgabe.Aufgabennr}.txt", CompressionLevel.Fastest);
                        using var entryStream = entry.Open();
                        await entryStream.WriteAsync(aufgabe.Input_TXT, 0, aufgabe.Input_TXT.Length);
                    }
                }
            }

            // ZIP in Base64 konvertieren
            var zipBytes = memoryStream.ToArray();
            return $"data:application/zip;base64,{Convert.ToBase64String(zipBytes)}";
        }

        private async Task UploadFile(IBrowserFile file, int aufgabenId)
        {
            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var uploadedFile = new UploadedFile(file.Name, memoryStream.ToArray());

            // Falls bereits eine Datei existiert, wird sie überschrieben
            UploadedFiles[aufgabenId] = uploadedFile;
        }

        private async Task SubmitFile(Aufgabe aufgabe)
        {
            if (UploadedFiles.TryGetValue(aufgabe.AufgabenID, out var uploadedFile))
            {
                string uploadedContent = System.Text.Encoding.UTF8.GetString(uploadedFile.FileData).Trim();
                string correctContent = System.Text.Encoding.UTF8.GetString(aufgabe.Ergebnis_TXT).Trim();

                bool isCorrect = uploadedContent == correctContent;
                if (!isCorrect && uploadedFile.FileIsRight == null)
                {
                    Fehlversuche++;
                }
                UploadedFiles[aufgabe.AufgabenID] = uploadedFile with { FileIsRight = isCorrect };
            }

            // Prüfen, ob alle Aufgaben eine richtige Datei haben
            if (CurrentLevel?.Aufgaben.Count == 0)
            {
                AllFilesSubmitted = true;
                return;
            }
            AllFilesSubmitted = CurrentLevel?.Aufgaben.All(a =>
                UploadedFiles.ContainsKey(a.AufgabenID) &&
                UploadedFiles[a.AufgabenID].FileIsRight == true) ?? false;
        }
        private async Task ProceedToNextLevel()
        {
            var absolviert = new GruppeAbsolviertLevel
            {
                Gruppe_GruppeID = Group.GruppenID,
                Level_LevelID = CurrentLevel.LevelID,
                Fehlversuche = Fehlversuche
            };

            dbContext.GruppeAbsolviertLevels.Add(absolviert);
            await dbContext.SaveChangesAsync();
            Fehlversuche = 0;

            Navigation.NavigateTo($"/participation/{EventId}");
        }
    }
    public record UploadedFile(string FileName, byte[] FileData, bool? FileIsRight = null);
}