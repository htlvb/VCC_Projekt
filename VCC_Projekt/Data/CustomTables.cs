namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System;
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

    // Entität für die Tabelle _event
    public class Event
    {
        public int EventID { get; set; }

        public string Bezeichnung { get; set; }

        public DateTime Beginn { get; set; }

        public int Dauer { get; set; }

        public int StrafminutenProFehlversuch { get; set; }

        // Navigation zu Gruppen
        public virtual ICollection<Gruppe> Gruppen { get; set; }

        // Navigation zu Levels
        public virtual ICollection<Level> Levels { get; set; }
    }

    // Entität für die Tabelle _level
    public class Level
    {
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

    // Entität für die Tabelle _aufgaben
    public class Aufgabe
    {
        public int AufgabenID { get; set; }

        public int Aufgabennr { get; set; }

        public byte[] Input_TXT { get; set; }

        public byte[] Ergebnis_TXT { get; set; }

        // Fremdschlüssel zu Level
        public int Level_LevelID { get; set; }

        // Navigation zu Level
        [ForeignKey("Level_LevelID")]
        public virtual Level Level { get; set; }
    }

    // Entität für die Tabelle _gruppe_absolviert_level
    public class GruppeAbsolviertLevel
    {
        // Fremdschlüssel zu Gruppe
        public int Gruppe_GruppeID { get; set; }
        [ForeignKey("Gruppe_GruppeID")]
        public virtual Gruppe Gruppe { get; set; }  // Navigation Property zu Gruppe

        // Fremdschlüssel zu Level
        [ForeignKey("Level_LevelID")]
        public int Level_LevelID { get; set; }
        public virtual Level Level { get; set; }  // Navigation Property zu Level

        public TimeSpan BenoetigteZeit { get; set; }
        public int Fehlversuche { get; set; }
    }




}