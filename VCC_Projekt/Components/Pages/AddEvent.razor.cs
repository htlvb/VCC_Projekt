using MailKit;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using VCC_Projekt.Components.Account.Pages.Manage;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddEvent
    {
        private List<Event> _events = new();
        private Event _selectedEvent = new() { EventID = 0 };
        private EditContext editContext;
        private bool isEditing = false;
        private InputModel Input { get; set; } = new();

        protected override void OnInitialized()
        {
            _events = dbContext.Events.OrderByDescending(ev => ev.Beginn).ToList();
            Input = new();
            editContext = new EditContext(Input);
            Input.Snackbar = Snackbar;
        }

        private void SetEventData()
        {
            Input.EventName = _selectedEvent.Bezeichnung;
            Input.EventDate = _selectedEvent.Beginn.Date;

            DateTime dateTime = DateTime.Parse(_selectedEvent.Beginn.ToString());
            TimeSpan timeSpan = dateTime.TimeOfDay;

            Input.StartTime = timeSpan;
            Input.EndTime = Input.StartTime?.Add(new TimeSpan(0, _selectedEvent.Dauer, 0));
            Input.PenaltyMinutes = _selectedEvent.StrafminutenProFehlversuch;

            StateHasChanged();
        }

        private void ToggleEditMode()
        {
            isEditing = !isEditing;
        }

        private async Task OnEventSelected(Event selectedEvent)
        {
            _selectedEvent = selectedEvent;
            SetEventData();
            if (isEditing == true) ToggleEditMode();
        }

        private async Task UpdateEvent()
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(Input);
                bool isValid = Validator.TryValidateObject(Input, validationContext, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        Console.WriteLine($"Validation Error: {validationResult.ErrorMessage}");
                    }
                    return;
                }

                try
                {
                    var eventToUpdate = dbContext.Events.Find(_selectedEvent.EventID);
                    if (eventToUpdate != null)
                    {
                        eventToUpdate.Bezeichnung = Input.EventName;
                        eventToUpdate.Beginn = DateTime.Parse(Input.EventDate?.Date.ToString("yyyy-MM-dd") + " " + Input.StartTime);
                        eventToUpdate.Dauer = (int)(Input.EndTime - Input.StartTime)?.TotalMinutes;
                        eventToUpdate.StrafminutenProFehlversuch = Input.PenaltyMinutes;

                        dbContext.SaveChanges();
                    }

                    ShowSnackbar("Wettbewerb wurde erfolgreich bearbeitet.", Severity.Success);
                    ToggleEditMode();
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowSnackbar("Fehler beim Bearbeiten des Wettbewerbs. Bitte versuche es erneut.", Severity.Error);
                Console.WriteLine($"Error during submission: {ex.Message}");
            }
        }

        private async Task CreateEvent()
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(Input);
                bool isValid = Validator.TryValidateObject(Input, validationContext, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        ShowSnackbar(validationResult.ErrorMessage, Severity.Error);
                        Console.WriteLine($"Validation Error: {validationResult.ErrorMessage}");
                    }
                    return;
                }

                Event ev = new Event();
                ev.Bezeichnung = Input.EventName;
                ev.Dauer = (int)(Input.EndTime - Input.StartTime)?.TotalMinutes;
                ev.StrafminutenProFehlversuch = Input.PenaltyMinutes;
                ev.Beginn = Input.EventDate.GetValueOrDefault(DateTime.Today).Date + Input.StartTime.GetValueOrDefault();

                dbContext.Events.Add(ev);
                dbContext.SaveChanges();

                ShowSnackbar("Wettbewerb wurde erfolgreich angelegt.", Severity.Success);

                Input = new InputModel();
            }
            catch (Exception ex)
            {
                ShowSnackbar("Fehler beim Anlegen des Wettbewerbs. Bitte versuche es erneut.", Severity.Error);
                Console.WriteLine($"Error during submission: {ex.Message}");
            }
        }

        private async Task DeleteEvent()
        {
            try
            {
                var eventToDelete = await dbContext.Events.FindAsync(_selectedEvent.EventID);
                if (eventToDelete != null)
                {
                    dbContext.Events.Remove(eventToDelete);
                    await dbContext.SaveChangesAsync();

                    ShowSnackbar("Wettbewerb wurde erfolgreich gelöscht.", Severity.Success);
                }
            }

            catch (Exception ex)
            {
                ShowSnackbar("Fehler beim Lösches des Wettbewerbs. Bitte versuche es erneut.", Severity.Error);
                Console.WriteLine($"Error during DeleteGroup: {ex.Message}");
            }
        }

        private async Task OpenEmailDialog()
        {
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters
            {
                { "UseDropdown", true },
                { "SelectedEmails", dbContext.UserInGruppe.Where(x=> x.Gruppe.Event_EventID == _selectedEvent.EventID).Select(x => x.User.Email).ToList()  },
                { "ReadOnly", true }
            };
            var dialog = await DialogService.ShowAsync<EmailSendDialog>($"Support Email Senden - Event {_selectedEvent.Bezeichnung}", parameters, options);
            var result = await dialog.Result;
        }

        private void ShowSnackbar(string message, Severity severity)
             => Snackbar.Add(message, severity);

        public sealed class InputModel : IValidatableObject
        {
            [DataType(DataType.Text)]
            [Display(Name = "Wettbewerbsname")]
            public string EventName { get; set; }

            [DataType(DataType.DateTime)]
            [Display(Name = "Datum")]
            public DateTime? EventDate { get; set; }

            [DataType(DataType.Time)]
            [Display(Name = "Startzeit")]
            public TimeSpan? StartTime { get; set; }

            [DataType(DataType.Time)]
            [Display(Name = "Endzeit")]
            public TimeSpan? EndTime { get; set; }

            [Display(Name = "Strafminuten")]
            [Range(0, int.MaxValue, ErrorMessage = "Strafminuten dürfen nicht negativ sein.")]
            public int PenaltyMinutes { get; set; } = 0;

            public ISnackbar Snackbar {  get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var errors = new List<ValidationResult>();
                bool isValid = true;

                // Validate EventName
                if (string.IsNullOrWhiteSpace(EventName))
                {
                    Snackbar?.Add("Bitte den Wettbewerbsnamen angeben.", Severity.Error);
                    isValid = false;
                }
                else if (EventName.Length < 3)
                {
                    Snackbar?.Add("Der Wettbewerbsname muss mindestens 3 Zeichen lang sein.", Severity.Error);
                    isValid = false;
                }

                // Validate Date and Time together
                var currentTime = DateTime.Now.TimeOfDay;
                var eventStart = EventDate?.Date + StartTime;
                var eventEnd = EventDate?.Date + EndTime;

                if (EventDate?.Date == null)
                {
                    Snackbar?.Add("Bitte ein Datum angeben.", Severity.Error);
                    isValid = false;
                }

                if (eventStart == null)
                {
                    Snackbar?.Add("Bitte eine Startzeit angeben.", Severity.Error);
                    isValid = false;
                }
                if (eventEnd == null)
                {
                    Snackbar?.Add("Bitte eine Endzeit angeben.", Severity.Error);
                    isValid = false;
                }


                // Check if date is in the past
                if (EventDate?.Date < DateTime.Today)
                {
                    Snackbar?.Add("Das Datum darf nicht in der Vergangenheit liegen.", Severity.Error);
                    isValid = false;
                }

                // If date is today, check time constraints
                else if (EventDate?.Date == DateTime.Today)
                {
                    if (StartTime < currentTime)
                    {
                        Snackbar?.Add("Die Startzeit muss in der Zukunft liegen, wenn das Event heute stattfindet.", Severity.Error); 
                        isValid = false;
                    }
                }

                // Validate End Time is after Start Time
                if (eventEnd <= eventStart)
                {
                    Snackbar?.Add("Die Endzeit muss nach der Startzeit liegen.", Severity.Error);
                    isValid = false;
                }

                if (!isValid)
                {
                    errors.Add(new ValidationResult("Snackbar-Fehler wurden angezeigt."));
                }

                return errors;
            }

            
        }

    }
}
