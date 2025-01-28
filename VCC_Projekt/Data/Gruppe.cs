namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    // Entität für die Tabelle _gruppe
    public class Gruppe
    {
        public int GruppenID { get; set; }

        public string Gruppenname { get; set; }

        // Fremdschlüssel für Event
        public int Event_EventID { get; set; }

        // Fremdschlüssel für Gruppenleiter
        public string GruppenleiterId { get; set; }  // Typ für IdentityUser (string)

        // Navigation Properties

        [ForeignKey("GruppenleiterId")]
        public virtual ApplicationUser GruppenleiterNavigation { get; set; }

        [ForeignKey("Event_EventID")]
        public virtual Event Event { get; set; }  // Navigation zu Event

        public string Teilnehmertyp { get; set; }

        // Beziehung zu 'GruppeAbsolviertLevel' 
        public ICollection<GruppeAbsolviertLevel> Absolviert { get; set; }

        public ICollection<ApplicationUser> Mitglieder { get; set; }
    }




}