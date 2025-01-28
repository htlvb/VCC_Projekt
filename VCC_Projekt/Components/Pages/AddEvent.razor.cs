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

        private InputModel Input { get; set; } = new();

        private async Task HandleSubmit()
        {
            // Logik zum Speichern des Wettbewerbs
            // Hier könnte eine API-Anfrage oder Datenbankoperation erfolgen
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "Bitte den Wettbewerbsnamen angeben.")]
            //mind. 3 zeichen
            [DataType(DataType.Text)]
            [Display(Name = "Wettbewerbsname")]
            public string EventName { get; set; }

            [Required(ErrorMessage = "Bitte ein Datum auswählen.")]
            //muss in der zukunft sein
            [DataType(DataType.DateTime)]
            [Display(Name = "Datum")]
            public DateTime EventDate { get; set; } = DateTime.Today;

            [Required(ErrorMessage = "Bitte eine Startzeit angeben.")]
            //kleiner als endzeit
            [DataType(DataType.Time)]
            [Display(Name = "Startzeit")]
            public TimeSpan StartTime { get; set; }

            [Required(ErrorMessage = "Bitte eine Endzeit angeben.")]
            //größer als startzeit
            [DataType(DataType.Time)]
            [Display(Name = "Endzeit")]
            public TimeSpan EndTime { get; set; }

            [Range(0, 60, ErrorMessage = "Bitte eine gültige Anzahl an Strafminuten angeben.")]
            // >= 0
            [Display(Name = "Strafminuten")]
            public int PenaltyMinutes { get; set; } = 0;
        }
    }
}
