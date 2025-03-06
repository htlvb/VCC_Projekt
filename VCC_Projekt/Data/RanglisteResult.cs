namespace VCC_Projekt.Data
{
    public class RanglisteResult
    {
        public int Rang { get; set; }
        public int GruppenID { get; set; }
        public string Gruppenname { get; set; }
        public int GruppenleiterId { get; set; }
        public string Teilnehmertyp { get; set; }
        public string AbgeschlosseneLevel { get; set; }
        public int AnzahlLevel { get; set; }
        public TimeSpan SchnellsteZeit { get; set; }
    }
}
