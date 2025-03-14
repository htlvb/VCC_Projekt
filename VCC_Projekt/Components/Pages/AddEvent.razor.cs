using Humanizer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using VCC_Projekt.Data;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddEvent
    {
        private List<Event> _events = new();
        private Event _selectedEvent = new() { EventID = 0 };
        private EditContext editContext;     
        private bool isEditing = false;

        protected override void OnInitialized()
        {
            _events = dbContext.Events.OrderByDescending(ev => ev.Beginn).ToList();
            Input = new();
            editContext = new EditContext(Input);
        }

        private void SetEventData()
        {
            Input.EventName = _selectedEvent.Bezeichnung;
            Input.EventDate = _selectedEvent.Beginn.Date;

            DateTime dateTime = DateTime.Parse(_selectedEvent.Beginn.ToString());
            TimeSpan timeSpan = dateTime.TimeOfDay;

            Input.StartTime = timeSpan;
            Input.EndTime = Input.StartTime.Add(new TimeSpan(0, _selectedEvent.Dauer, 0));
            Input.PenaltyMinutes = _selectedEvent.StrafminutenProFehlversuch;
        }

        private void ToggleEditMode()
        {
            isEditing = !isEditing;
        }

        private async Task OnEventSelected(Event selectedEvent)
        {
            _selectedEvent = selectedEvent;
            _events = dbContext.Events.OrderByDescending(ev => ev.Beginn).ToList();
            SetEventData();
            if (isEditing == true) ToggleEditMode();
        }

        private void UpdateStartTime(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
            {
                Input.StartTime = time;
                editContext.NotifyFieldChanged(FieldIdentifier.Create(() => Input.StartTime));
            }
        }

        private void UpdateEndTime(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
            {
                Input.EndTime = time;
                editContext.NotifyFieldChanged(FieldIdentifier.Create(() => Input.EndTime));
            }
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
                    int rowsEffected = dbContext.Database.ExecuteSqlRaw($"UPDATE vcc_event SET Bezeichnung = '{Input.EventName}', Beginn = '{Input.EventDate.Date.ToString("yyyy-MM-dd") + " " + Input.StartTime}', Dauer = {(int)(Input.EndTime - Input.StartTime).TotalMinutes}, StrafminutenProFehlversuch = {Input.PenaltyMinutes} WHERE EventID = {_selectedEvent.EventID}" );
                    ShowSnackbar("Wettbewerb wurde erfolgreich bearbeitet.", Severity.Success);
                    ToggleEditMode();

                    //Refresh
                }

                catch(Exception ex)
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
                        Console.WriteLine($"Validation Error: {validationResult.ErrorMessage}");
                    }
                    return;
                }

                Event ev = new Event();
                ev.Bezeichnung = Input.EventName;
                ev.Dauer = (int)(Input.EndTime - Input.StartTime).TotalMinutes;
                ev.StrafminutenProFehlversuch = Input.PenaltyMinutes;
                ev.Beginn = Input.EventDate.Date + Input.StartTime;

                dbContext.Events.Add(ev);
                dbContext.SaveChanges();

                ShowSnackbar("Wettbewerb wurde erfolgreich angelegt.", Severity.Success);

                Input = new InputModel();

                //Refresh
            }
            catch (Exception ex)
            {
                ShowSnackbar("Fehler beim Anlegen des Wettbewerbs. Bitte versuche es erneut.", Severity.Error);
                Console.WriteLine($"Error during submission: {ex.Message}");
            }
        }

        private void ShowSnackbar(string message, Severity severity)
             => Snackbar.Add(message, severity);

        private InputModel Input { get; set; } = new();

        public sealed class InputModel : IValidatableObject
        {
            [DataType(DataType.Text)]
            [Display(Name = "Wettbewerbsname")]
            public string EventName { get; set; }

            [DataType(DataType.DateTime)]
            [Display(Name = "Datum")]
            public DateTime EventDate { get; set; } = DateTime.Today;

            [DataType(DataType.Time)]
            [Display(Name = "Startzeit")]
            public TimeSpan StartTime { get; set; }

            [DataType(DataType.Time)]
            [Display(Name = "Endzeit")]
            public TimeSpan EndTime { get; set; }

            [Display(Name = "Strafminuten")]
            [Range(0, int.MaxValue, ErrorMessage = "Strafminuten dürfen nicht negativ sein.")]
            public int PenaltyMinutes { get; set; } = 0;

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var errors = new List<ValidationResult>();

                // Validate EventName
                if (string.IsNullOrWhiteSpace(EventName))
                {
                    errors.Add(new ValidationResult("Bitte den Wettbewerbsnamen angeben.", new[] { nameof(EventName) }));
                }
                else if (EventName.Length < 3)
                {
                    errors.Add(new ValidationResult("Der Wettbewerbsname muss mindestens 3 Zeichen lang sein.", new[] { nameof(EventName) }));
                }

                // Validate Date and Time together
                var currentTime = DateTime.Now.TimeOfDay;
                var eventStart = EventDate.Date + StartTime;
                var eventEnd = EventDate.Date + EndTime;

                // Check if date is in the past
                if (EventDate.Date < DateTime.Today)
                {
                    errors.Add(new ValidationResult("Das Datum darf nicht in der Vergangenheit liegen.", new[] { nameof(EventDate) }));
                }
                // If date is today, check time constraints
                else if (EventDate.Date == DateTime.Today)
                {
                    if (StartTime < currentTime)
                    {
                        errors.Add(new ValidationResult("Die Startzeit muss in der Zukunft liegen, wenn das Event heute stattfindet.", new[] { nameof(StartTime) }));
                    }
                }

                // Validate End Time is after Start Time
                if (eventEnd <= eventStart)
                {
                    errors.Add(new ValidationResult("Die Endzeit muss nach der Startzeit liegen.", new[] { nameof(EndTime) }));
                }

                return errors;
            }
        }

    }
}
