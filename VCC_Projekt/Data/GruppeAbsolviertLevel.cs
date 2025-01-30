namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    // Entität für die Tabelle _gruppe_absolviert_level
    public class GruppeAbsolviertLevel
    {
        public GruppeAbsolviertLevel(int gruppe_GruppeID, Gruppe gruppe, int level_LevelID, int fehlversuche)
        {
            Gruppe_GruppeID = gruppe_GruppeID;
            Gruppe = gruppe;
            Level_LevelID = level_LevelID;
            Fehlversuche = fehlversuche;
        }

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