namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    // Entität für die Tabelle _level
    public class Level
    {
        public Level(byte[] angabe_PDF, int event_EventID)
        {
            Angabe_PDF = angabe_PDF;
            Event_EventID = event_EventID;
        }

        public int LevelID { get; set; }

        public int Levelnr { get; set; }

        public byte[] Angabe_PDF { get; set; }

        // Fremdschlüssel zu Event
        public int Event_EventID { get; set; }

        // Navigation zu Event
        [ForeignKey("Event_EventID")]
        public virtual Event Event { get; set; }

        // Beziehung zu 'GruppeAbsolviertLevel'
        public virtual ICollection<GruppeAbsolviertLevel> Absolviert { get; set; }
    }




}