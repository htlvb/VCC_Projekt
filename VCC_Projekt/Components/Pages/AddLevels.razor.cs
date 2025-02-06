using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddLevels
    {
        private List<LevelViewModel> _levels = new();
        private List<Event> _events = new();
        private int _selectedEventId;
        private bool IsEventInPast => _events.FirstOrDefault(e => e.EventID == _selectedEventId)?.Beginn < DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            _events = await dbContext.Events.OrderByDescending(ev => ev.Beginn).ToListAsync();
        }

        private async Task OnEventSelected(int eventId)
        {
            _selectedEventId = eventId;

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
                    Aufgaben = level.Aufgaben,
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
                _levels.Add(new LevelViewModel { Levelnr = _levels.Count + 1, Event_EventID = _selectedEventId, Aufgaben = new List<Aufgabe>(), IsExpanded = false });
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
                _levels[levelIndex].Aufgaben.Add(new Aufgabe { Aufgabennr = _levels[levelIndex].Aufgaben.Count + 1 });
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

        private async Task UploadFile(IBrowserFile file, int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && !IsEventInPast)
            {
                if (file != null)
                {
                    using var memoryStream = new MemoryStream();
                    await file.OpenReadStream().CopyToAsync(memoryStream);
                    _levels[levelIndex].Angabe_PDF = memoryStream.ToArray();
                }
            }
        }

        private async Task UploadTaskFile(IBrowserFile file, int levelIndex, int taskIndex, string type)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count && taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count && !IsEventInPast)
            {
                if (file != null)
                {
                    using var memoryStream = new MemoryStream();
                    await file.OpenReadStream().CopyToAsync(memoryStream);

                    if (type == "input")
                        _levels[levelIndex].Aufgaben[taskIndex].Input_TXT = memoryStream.ToArray();
                    else if (type == "output")
                        _levels[levelIndex].Aufgaben[taskIndex].Ergebnis_TXT = memoryStream.ToArray();
                }
            }
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
                    var existingLevel = existingLevels.FirstOrDefault(l => l.Levelnr == level.Levelnr);
                    if (existingLevel != null)
                    {
                        existingLevel.Angabe_PDF = level.Angabe_PDF;
                        existingLevel.Aufgaben = level.Aufgaben;
                    }
                    else
                    {
                        dbContext.Levels.Add(new Level
                        {
                            Levelnr = level.Levelnr,
                            Event_EventID = level.Event_EventID,
                            Angabe_PDF = level.Angabe_PDF,
                            Aufgaben = level.Aufgaben
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
                Snackbar.Add($"Fehler beim Speichern der Levels: {ex.Message}", Severity.Error, config =>
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
        public List<Aufgabe> Aufgaben { get; set; }
        public bool IsExpanded { get; set; } // This property is for UI state
    }


}
