using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;

    public class Event
    {
        public int EventID { get; set; }
        public string Bezeichnung { get; set; }
        public DateTime Beginn { get; set; }
        public int Dauer { get; set; }
        public int StrafminutenProFehlversuch { get; set; }
    }

    public class Level
    {
        public int LevelID { get; set; }
        public int Levelnr { get; set; }
        public byte[] AngabePDF { get; set; }
        public int EventID { get; set; }
    }

    public class GroupCompletedLevel
    {
        public int GruppeID { get; set; }
        public int LevelID { get; set; }
        public TimeSpan BenoetigteZeit { get; set; }
        public int Fehlversuche { get; set; }
    }

    public class Task
    {
        public int AufgabenID { get; set; }
        public int Aufgabennr { get; set; }
        public byte[] InputTXT { get; set; }
        public byte[] ErgebnisTXT { get; set; }
        public int LevelID { get; set; }
    }






}