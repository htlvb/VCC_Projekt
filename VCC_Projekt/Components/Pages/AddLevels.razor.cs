using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddLevels
    {
        private List<LevelViewModel> _levels = new();
        private List<Event> _events = new();
        private int _selectedEventId;
        private Event _selectedEvent;
        private bool IsEventInPast => _events.FirstOrDefault(e => e.EventID == _selectedEventId)?.Beginn < DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            _events = await dbContext.Events.OrderByDescending(ev => ev.Beginn).ToListAsync();
        }

        private async Task OnEventSelected(Event eventId)
        {
            _selectedEventId = eventId.EventID;
            _selectedEvent =  eventId;

            if (_selectedEventId != 0)
            {
                var levels = await dbContext.Levels
                                            .Where(l => l.Event_EventID == _selectedEventId)
                                            .Include(l => l.Aufgaben)
                                            .ToListAsync();

                _levels = levels.Select(level => new LevelViewModel
                {
                    Levelnr = level.Levelnr,
                    Event_EventID = level.Event_EventID,
                    Angabe_PDF = level.Angabe_PDF,
                    Aufgaben = level.Aufgaben.Select(aufgabe => new AufgabeViewModel
                    {
                        Aufgabennr = aufgabe.Aufgabennr,
                        Input_TXT = aufgabe.Input_TXT,
                        Ergebnis_TXT = aufgabe.Ergebnis_TXT,
                        IsExpanded = false
                    }).ToList(),
                    IsExpanded = false
                }).ToList();
            }
            else
            {
                _levels.Clear();
            }
        }

        private void AddLevel()
        {
            if (_levels.Count < 5 && _selectedEventId != 0 && !IsEventInPast)
            {
                _levels.Add(new LevelViewModel { Levelnr = _levels.Count + 1, Event_EventID = _selectedEventId, Aufgaben = new List<AufgabeViewModel>(), IsExpanded = false });
            }
        }

        private void RemoveLevel(int index)
        {
            if (index >= 0 && index < _levels.Count && !IsEventInPast)
            {
                _levels.RemoveAt(index);
                ReorderLevels();
            }
        }

        private void AddTask(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && !IsEventInPast)
            {
                _levels[levelIndex].Aufgaben.Add(new AufgabeViewModel { Aufgabennr = _levels[levelIndex].Aufgaben.Count + 1, IsExpanded = false });
            }
        }

        private void RemoveTask(int levelIndex, int taskIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count &&
                taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count && !IsEventInPast)
            {
                _levels[levelIndex].Aufgaben.RemoveAt(taskIndex);
            }
        }

        private void ToggleLevel(int index)
        {
            if (index >= 0 && index < _levels.Count)
            {
                _levels[index].IsExpanded = !_levels[index].IsExpanded;
            }
        }

        private void ToggleTask(int levelIndex, int taskIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count)
            {
                _levels[levelIndex].Aufgaben[taskIndex].IsExpanded = !_levels[levelIndex].Aufgaben[taskIndex].IsExpanded;
            }
        }

        private async Task UploadFile(IBrowserFile file, int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && !IsEventInPast)
            {
                if (file != null)
                {
                    _levels[levelIndex].Angabe_PDF = await ConvertToBytesAsync(file);
                }
            }
        }

        

        private async Task UploadTaskFile(IBrowserFile file, int levelIndex, int taskIndex, string type)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count && !IsEventInPast)
            {
                if (file != null)
                {

                    if (type == "input")
                        _levels[levelIndex].Aufgaben[taskIndex].Input_TXT = await ConvertToBytesAsync(file);
                    else if (type == "output")
                        _levels[levelIndex].Aufgaben[taskIndex].Ergebnis_TXT = await ConvertToBytesAsync(file);
                }
            }
            Snackbar.Add("Datei erfolgreich hochgeladen!", Severity.Success, config =>
            {
                config.Icon = Icons.Material.Filled.CheckCircle;
            });
            StateHasChanged();
        }

        private async Task<byte[]> ConvertToBytesAsync(IBrowserFile file)
        {
            const long maxFileSize = 5 * 1024 * 1024;
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private async Task SaveLevels()
        {
            if (_selectedEventId == 0 || IsEventInPast)
                return;

            try
            {
                // Retrieve existing levels from the database
                var existingLevels = await dbContext.Levels
                                                    .Where(l => l.Event_EventID == _selectedEventId)
                                                    .ToListAsync();

                // Update existing levels and add new levels
                foreach (var level in _levels)
                {
                    if (level.Angabe_PDF == null)
                    {
                        throw new ArgumentException($"Es muss eine PDF bei Level {level.Levelnr} hochgeladen werden!");
                    }

                    foreach (var aufgabe in level.Aufgaben)
                    {
                        if (aufgabe.Input_TXT == null || aufgabe.Ergebnis_TXT == null)
                        {
                            throw new ArgumentException($"Die Aufgabe {aufgabe.Aufgabennr} im Level {level.Levelnr} muss sowohl eine Input.txt als auch eine Output.txt Datei haben!");
                        }
                    }

                    var existingLevel = existingLevels.FirstOrDefault(l => l.Levelnr == level.Levelnr);
                    if (existingLevel != null)
                    {
                        existingLevel.Angabe_PDF = level.Angabe_PDF;
                        existingLevel.Aufgaben = level.Aufgaben.Select(aufgabe => new Aufgabe
                        {
                            Aufgabennr = aufgabe.Aufgabennr,
                            Input_TXT = aufgabe.Input_TXT,
                            Ergebnis_TXT = aufgabe.Ergebnis_TXT
                        }).ToList();
                    }
                    else
                    {
                        dbContext.Levels.Add(new Level
                        {
                            Levelnr = level.Levelnr,
                            Event_EventID = level.Event_EventID,
                            Angabe_PDF = level.Angabe_PDF,
                            Aufgaben = level.Aufgaben.Select(aufgabe => new Aufgabe
                            {
                                Aufgabennr = aufgabe.Aufgabennr,
                                Input_TXT = aufgabe.Input_TXT,
                                Ergebnis_TXT = aufgabe.Ergebnis_TXT
                            }).ToList()
                        });
                    }
                }

                // Remove levels that are no longer present
                var levelsToRemove = existingLevels.Where(existingLevel => !_levels.Any(level => level.Levelnr == existingLevel.Levelnr)).ToList();
                dbContext.Levels.RemoveRange(levelsToRemove);

                await dbContext.SaveChangesAsync();
                Snackbar.Add("Levels erfolgreich gespeichert!", Severity.Success, config =>
                {
                    config.Icon = Icons.Material.Filled.CheckCircle;
                });
            }
            catch (Exception ex)
            {
                string result = ex.InnerException == null ? ex.Message : ex.InnerException.ToString();
                Snackbar.Add($"Fehler beim Speichern der Levels: {ex.InnerException?.ToString() ?? ex.Message}", Severity.Error, config =>
                {
                    config.Icon = Icons.Material.Filled.Error;
                });
            }
        }

        private void ReorderLevels()
        {
            for (int i = 0; i < _levels.Count; i++)
            {
                _levels[i].Levelnr = i + 1;
            }
        }
    }

    public class LevelViewModel
    {
        public int Levelnr { get; set; }
        public int Event_EventID { get; set; }
        public byte[] Angabe_PDF { get; set; }
        public List<AufgabeViewModel> Aufgaben { get; set; }
        public bool IsExpanded { get; set; }
    }

    public class AufgabeViewModel
    {
        public int Aufgabennr { get; set; }
        public byte[] Input_TXT { get; set; }
        public byte[] Ergebnis_TXT { get; set; }
        public bool IsExpanded { get; set; }
    }


}
