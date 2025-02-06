using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddEvent
    {
        private EditContext editContext;

        protected override void OnInitialized()
        {
            Input = new();
            editContext = new EditContext(Input);
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

        private async Task HandleSubmit()
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

                await SaveCompetition(Input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during submission: {ex.Message}");
            }
        }

        private async Task SaveCompetition(InputModel model)
        {
            try
            {
                int duration = (int)(model.EndTime - model.StartTime).TotalMinutes;
                DateTime eventStartTime = model.EventDate.Date + model.StartTime;

                Event ev = new Event();

                ev.Bezeichnung = Input.EventName;
                ev.Dauer = duration;
                ev.StrafminutenProFehlversuch = Input.PenaltyMinutes;
                ev.Beginn = eventStartTime;

                dbContext.Events.Add(ev);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

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

                // Validate PenaltyMinutes
                if (PenaltyMinutes < 0)
                {
                    errors.Add(new ValidationResult("Die Strafminuten dürfen nicht negativ sein.", new[] { nameof(PenaltyMinutes) }));
                }

                return errors;
            }
        }

    }
}
