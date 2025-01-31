namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations.Schema;

public class Aufgabe
{

    public Aufgabe()
    {
    }

    public Aufgabe(byte[] input_TXT, byte[] ergebnis_TXT, int level_LevelID)
    {
        AufgabenID = aufgabenID;
        Aufgabennr = aufgabennr;
        Input_TXT = input_TXT;
        Ergebnis_TXT = ergebnis_TXT;
        Level_LevelID = level_LevelID;
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

public class AufgabenConfiguration : IEntityTypeConfiguration<Aufgabe>
{
    public void Configure(EntityTypeBuilder<Aufgabe> builder)
    {
        builder.ToTable("vcc_aufgaben");
        builder.HasKey(t => t.AufgabenID);

        // Korrekte Beziehung zu Level
        builder.HasOne(t => t.Level)
              .WithMany()
              .HasForeignKey(t => t.Level_LevelID)
              .HasPrincipalKey(l => l.LevelID);
    }
}
