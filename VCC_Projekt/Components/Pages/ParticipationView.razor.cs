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

        protected override async void OnInitialized()
        {
            isLoading = true;
            try
            {
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
                                     }).OrderBy(le => le.Levelnr).ToList()
                                 })
                                 .FirstOrDefault();

                if (EventId <= 0 || Event == null) throw new ArgumentException("Event nicht gefunden");

                DateTime now = DateTime.Now;
                // if (!(now >= Event.Beginn && now <= Event.Beginn.AddMinutes(Event.Dauer))) throw new ArgumentException("Event wird aktuell nicht ausgeführt");

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

                User = authState.User;

                if (User.Identity == null || !User.Identity.IsAuthenticated) throw new ArgumentException("Benutzer nicht gefunden");

                Group = dbContext.Gruppen
                    .Where(gr => gr.Event_EventID == EventId && gr.UserInGruppe.Any(us => us.User_UserId == User.Identity.Name))
                    .Include(g => g.Absolviert)
                    .Include(n => n.UserInGruppe)
                    .FirstOrDefault();

                if (Group == null) throw new ArgumentException("Gruppe nicht gefunden");

                if (Group.Gesperrt) throw new ArgumentException("Gruppe gesperrt");

                CurrentLevel = dbContext.Levels
                                        .Where(level => level.Event_EventID == EventId)
                                        .Where(level => !dbContext.GruppeAbsolviertLevels
                                            .Any(a => a.Level_LevelID == level.LevelID && a.Gruppe_GruppeID == Group.GruppenID))
                                        .OrderBy(l => l.Levelnr)
                                        .Include(l => l.Aufgaben)
                                        .FirstOrDefault();

                if (CurrentLevel == null) throw new ArgumentException("Kein Level gefunden");

                if (CurrentLevel.Aufgaben.Count == 0) AllFilesSubmitted = true;
            }
            catch (Exception ex)
            {
                accessDenied = true;
                accessDeniedMessage = ex.Message;
            }
            
            isLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (accessDenied)
                {
                    if (Event != null)
                    {
                        await ProtectedLocalStorage.DeleteAsync($"Fehlversuche_{Event.EventID}"); // Löscht die Fehlversuche aus dem Local Storage für das Event
                    }
                    return;
                }

                await JS.InvokeVoidAsync("startTimer");

                var result = await ProtectedLocalStorage.GetAsync<int>($"Fehlversuche_{Event.EventID}"); //Holt die Fehlversuche des Events aus dem Local Storage des Browsers
                Fehlversuche = result.Success ? result.Value : 0;

                StateHasChanged(); // UI-Update auslösen
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
                        var entry = archive.CreateEntry($"level{CurrentLevel.Levelnr}_{aufgabe.Aufgabennr}.in", CompressionLevel.Fastest);
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
                await ProtectedLocalStorage.SetAsync($"Fehlversuche_{Event.EventID}",Fehlversuche);
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

            await ProtectedLocalStorage.DeleteAsync($"Fehlversuche_{Event.EventID}"); //Löscht den Localen Storage des Browsers für das Event

            Navigation.NavigateTo($"/participation/{EventId}");
        }
    }
    public record UploadedFile(string FileName, byte[] FileData, bool? FileIsRight = null);
}