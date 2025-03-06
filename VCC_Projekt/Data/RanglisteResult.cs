using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VCC_Projekt.Data
{
    public class RanglisteResult
    {
        public int Rang { get; set; }
        public int GruppenID { get; set; }
        public string? Gruppenname { get; set; }
        public string? GruppenleiterId { get; set; }
        public string Teilnehmertyp { get; set; }
        public string AbgeschlosseneLevel { get; set; }
        public int AnzahlLevel { get; set; }
        public int GesamtFehlversuche { get; set; } // Neue Spalte: Gesamtzahl der Fehlversuche
        public TimeSpan? MaxBenötigteZeit { get; set; } // Neue Spalte: Maximale benötigte Zeit
        public TimeSpan? GebrauchteZeit { get; set; } // Gesamtzeit inkl. Strafzeit
    }

    public class RanglisteConfiguration : IEntityTypeConfiguration<RanglisteResult>
    {
        public void Configure(EntityTypeBuilder<RanglisteResult> builder)
        {
            builder.HasNoKey();
        }
    }



}
