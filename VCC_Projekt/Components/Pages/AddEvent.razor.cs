using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddEvent
    {
        private void UpdateStartTime(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
            {
                Input.StartTime = time;
            }
        }

        private void UpdateEndTime(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
            {
                Input.EndTime = time;
            }
            
        }


        private async Task<(bool isValid, string[] errors)> ValidateInput(InputModel model)
        {
            var errors = new List<string>();
            bool isValid = true;

            // Validate EventName
            if (string.IsNullOrWhiteSpace(model.EventName))
            {
                errors.Add("Bitte den Wettbewerbsnamen angeben.");
                isValid = false;
            }
            else if (model.EventName.Length < 3)
            {
                errors.Add("Der Wettbewerbsname muss mindestens 3 Zeichen lang sein.");
                isValid = false;
            }

            // Validate Date and Time together
            var currentTime = DateTime.Now.TimeOfDay;
            var eventStart = model.EventDate.Date + model.StartTime;
            var eventEnd = model.EventDate.Date + model.EndTime;

            // Check if date is in the past
            if (model.EventDate.Date < DateTime.Today)
            {
                errors.Add("Das Datum darf nicht in der Vergangenheit liegen.");
                isValid = false;
            }
            // If date is today, check time constraints
            else if (model.EventDate.Date == DateTime.Today)
            {
                if (model.StartTime < currentTime)
                {
                    errors.Add("Die Startzeit muss in der Zukunft liegen wenn das Event heute stattfindet.");
                    isValid = false;
                }
            }

            // Validate End Time is after Start Time
            if (eventEnd <= eventStart)
            {
                errors.Add("Die Endzeit muss nach der Startzeit liegen.");
                isValid = false;
            }

            // Validate PenaltyMinutes
            if (model.PenaltyMinutes < 0)
            {
                errors.Add("Die Strafminuten dürfen nicht negativ sein.");
                isValid = false;
            }

            return (isValid, errors.ToArray());
        }

        private async Task HandleSubmit()
        {
            try
            {
                // Assuming we have access to the current InputModel instance as 'model'
                var (isValid, errors) = await ValidateInput(Input);

                if (!isValid)
                {
                    // Handle validation errors
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error}");
                    }
                    return;
                }

                // If validation passes, proceed with saving the competition
                await SaveCompetition(Input);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during submission
                Console.WriteLine($"Error during submission: {ex.Message}");
                // Add appropriate error handling for your application
            }
        }

        private async Task SaveCompetition(InputModel model)
        {
            // Implement your competition saving logic here
            // This could include:
            // - API calls
            // - Database operations
            // - State updates

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
                Console.WriteLine("Error");
            }
        }
        private InputModel Input { get; set; } = new();

        private sealed class InputModel
        {
            [Required(ErrorMessage = "Bitte den Wettbewerbsnamen angeben.")]
            [MinLength(3, ErrorMessage = "Der Wettbewerbsname muss mindestens 3 Zeichen lang sein.")]
            [DataType(DataType.Text)]
            [Display(Name = "Wettbewerbsname")]
            public string EventName { get; set; }

            [Required(ErrorMessage = "Bitte ein Datum auswählen.")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Datum")]
            public DateTime EventDate { get; set; } = DateTime.Today;

            [Required(ErrorMessage = "Bitte eine Startzeit angeben.")]
            [DataType(DataType.Time)]
            [Display(Name = "Startzeit")]
            public TimeSpan StartTime { get; set; }

            [Required(ErrorMessage = "Bitte eine Endzeit angeben.")]
            [DataType(DataType.Time)]
            [Display(Name = "Endzeit")]
            public TimeSpan EndTime { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Die Strafminuten dürfen nicht negativ sein.")]
            [Display(Name = "Strafminuten")]
            public int PenaltyMinutes { get; set; } = 0;
        }
    }
}
