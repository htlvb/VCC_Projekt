using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddLevels
    {
        private List<LevelViewModel> _levels = new();
        private List<Event> _events = new();
        private int _selectedEventId;

        protected override async Task OnInitializedAsync()
        {
            _events = await dbContext.Events.ToListAsync();
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
            if (_levels.Count < 5 && _selectedEventId != 0)
            {
                _levels.Add(new LevelViewModel { Levelnr = _levels.Count + 1, Event_EventID = _selectedEventId, Aufgaben = new List<Aufgabe>(), IsExpanded = false });
            }
        }

        private void RemoveLevel(int index)
        {
            if (index >= 0 && index < _levels.Count)
            {
                _levels.RemoveAt(index);
            }
        }

        private void AddTask(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count)
            {
                _levels[levelIndex].Aufgaben.Add(new Aufgabe { Aufgabennr = _levels[levelIndex].Aufgaben.Count + 1 });
            }
        }

        private void RemoveTask(int levelIndex, int taskIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count &&
                taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count)
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
            if (levelIndex >= 0 && levelIndex < _levels.Count)
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
            if (levelIndex >= 0 && levelIndex < _levels.Count && taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count)
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
            if (_selectedEventId == 0)
                return;

            var levels = _levels.Select(level => new Level
            {
                Levelnr = level.Levelnr,
                Event_EventID = level.Event_EventID,
                Angabe_PDF = level.Angabe_PDF,
                Aufgaben = level.Aufgaben
            }).ToList();

            dbContext.Levels.UpdateRange(levels);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Levels gespeichert!");
        }

        private void OnOrderChanged()
        {
            // Handle order change if needed
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

    
}
