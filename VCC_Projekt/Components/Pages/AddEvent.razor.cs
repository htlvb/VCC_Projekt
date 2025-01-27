using static VCC_Projekt.Components.Account.Pages.Register;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class AddEvent
    {
        private InputModel Input { get; set; } = new();

        private async Task HandleSubmit()
        {
            // Logik zum Speichern des Wettbewerbs
            // Hier könnte eine API-Anfrage oder Datenbankoperation erfolgen
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "Bitte den Wettbewerbsnamen angeben.")]
            [DataType(DataType.Text)]
            public string EventName { get; set; }

            [Required(ErrorMessage = "Bitte ein Datum auswählen.")]
            [DataType(DataType.DateTime)]
            public DateTime EventDate { get; set; }

            [Required(ErrorMessage = "Bitte eine Startzeit angeben.")]
            [DataType(DataType.Time)]
            public TimeSpan StartTime { get; set; }

            [Required(ErrorMessage = "Bitte eine Endzeit angeben.")]
            [DataType(DataType.Time)]
            public TimeSpan EndTime { get; set; }

            [Range(0, 60, ErrorMessage = "Bitte eine gültige Anzahl an Strafminuten angeben.")]
            public int PenaltyMinutes { get; set; }
        }
    }
}
