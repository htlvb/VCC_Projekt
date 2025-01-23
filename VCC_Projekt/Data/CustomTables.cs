using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Security.Claims;


    // Entität für die Tabelle _gruppe
    public class Gruppe
    {
        public int GruppenID { get; set; }

        public string Gruppenname { get; set; }

        // Fremdschlüssel für Event
        public int EventID { get; set; }

        // Fremdschlüssel für Gruppenleiter
        public string Gruppenleiter { get; set; }  // Typ für IdentityUser (string)

        // Navigation Properties
        public virtual ApplicationUser GruppenleiterNavigation { get; set; }
        public virtual Event Event { get; set; }  // Navigation zu Event

        public string Teilnehmertyp { get; set; }

        // Beziehung zu 'GruppeAbsolviertLevel' 
        [ForeignKey("Gruppen_Gruppen")]
        public ICollection<GruppeAbsolviertLevel> Absolviert { get; set; }
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
        public int EventID { get; set; }

        // Navigation zu Event
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
        public int LevelID { get; set; }

        // Navigation zu Level
        public virtual Level Level { get; set; }
    }

    // Entität für die Tabelle _gruppe_absolviert_level
    public class GruppeAbsolviertLevel
    {
        // Fremdschlüssel zu Gruppe
        public int GruppeID { get; set; }
        public virtual Gruppe Gruppe { get; set; }  // Navigation Property zu Gruppe

        // Fremdschlüssel zu Level
        public int LevelID { get; set; }
        public virtual Level Level { get; set; }  // Navigation Property zu Level

        public TimeSpan BenoetigteZeit { get; set; }
        public int Fehlversuche { get; set; }
    }




}