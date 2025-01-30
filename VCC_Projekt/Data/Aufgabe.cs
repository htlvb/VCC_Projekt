namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using System.ComponentModel.DataAnnotations.Schema;

    // Entität für die Tabelle _aufgaben
    public class Aufgabe
    {
        public Aufgabe(byte[] input_txt, byte[] ergebnis_txt, int level_levelId)
        {
            Input_TXT = input_txt;
            Ergebnis_TXT = ergebnis_txt;
            Level_LevelID = level_levelId;
        }
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




}