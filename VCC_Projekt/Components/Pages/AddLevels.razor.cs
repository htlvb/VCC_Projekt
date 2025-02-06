//using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.EntityFrameworkCore;

//namespace VCC_Projekt.Components.Pages
//{
//    public partial class AddLevels
//    {
//        private List<Level> _levels = new();
//        private List<Event> _events = new();
//        private int _selectedEventId;

//        protected override async Task OnInitializedAsync()
//        {
//            _events = await dbContext.Events.ToListAsync();
//        }

//        // Event wird geändert
//        private async Task OnEventSelected(int eventId)
//        {
//            _selectedEventId = eventId;

//            // Levels für das ausgewählte Event laden
//            if (_selectedEventId != 0)
//            {
//                _levels = await dbContext.Levels
//                                          .Where(l => l.Event_EventID == _selectedEventId)
//                                          .Include(l => l.Aufgaben)  // Falls Aufgaben ebenfalls geladen werden sollen
//                                          .ToListAsync();
//            }
//            else
//            {
//                _levels.Clear(); // Falls kein Event ausgewählt ist, Levels löschen
//            }
//        }

//        private void AddLevel()
//        {
//            if (_levels.Count < 5 && _selectedEventId != 0)
//                _levels.Add(new Level { Levelnr = _levels.Count + 1, Event_EventID = _selectedEventId, Aufgaben = new List<Aufgabe>() });
//        }

//        private void RemoveLevel(int index)
//        {
//            if (index >= 0 && index < _levels.Count)
//                _levels.RemoveAt(index);
//        }

//        private void AddTask(int levelIndex)
//        {
//            if (levelIndex >= 0 && levelIndex < _levels.Count)
//            {
//                _levels[levelIndex].Aufgaben.Add(new Aufgabe { Aufgabennr = _levels[levelIndex].Aufgaben.Count + 1 });
//            }
//        }

//        private void RemoveTask(int levelIndex, int taskIndex)
//        {
//            if (levelIndex >= 0 && levelIndex < _levels.Count &&
//                taskIndex >= 0 && taskIndex < _levels[levelIndex].Aufgaben.Count)
//            {
//                _levels[levelIndex].Aufgaben.RemoveAt(taskIndex);
//            }
//        }

//        private async Task UploadFile(IEnumerable<IBrowserFile> files, int levelIndex)
//        {
//            if (files != null && files.Any() && levelIndex < _levels.Count)
//            {
//                using var memoryStream = new MemoryStream();
//                await files.First().OpenReadStream().CopyToAsync(memoryStream);
//                _levels[levelIndex].Angabe_PDF = memoryStream.ToArray();
//            }
//        }

//        private async Task UploadTaskFile(IEnumerable<IBrowserFile> files, int levelIndex, int taskIndex, string type)
//        {
//            if (files != null && files.Any() && levelIndex < _levels.Count && taskIndex < _levels[levelIndex].Aufgaben.Count)
//            {
//                using var memoryStream = new MemoryStream();
//                await files.First().OpenReadStream().CopyToAsync(memoryStream);

//                if (type == "input")
//                    _levels[levelIndex].Aufgaben[taskIndex].Input_TXT = memoryStream.ToArray();
//                else if (type == "output")
//                    _levels[levelIndex].Aufgaben[taskIndex].Ergebnis_TXT = memoryStream.ToArray();
//            }
//        }

//        private async Task SaveLevels()
//        {
//            if (_selectedEventId == 0)
//                return;

//            foreach (var level in _levels)
//            {
//                level.Event_EventID = _selectedEventId;
//            }

//            dbContext.Levels.AddRange(_levels);
//            await dbContext.SaveChangesAsync();
//            Console.WriteLine("Levels gespeichert!");
//        }



//    }
//}
