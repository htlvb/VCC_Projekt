namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public class GruppeAbsolviertLevel
{
    public GruppeAbsolviertLevel() { }
    public GruppeAbsolviertLevel(int gruppe_GruppeID, int level_LevelID, int fehlversuche)
    {
        Gruppe_GruppeID = gruppe_GruppeID;
        Level_LevelID = level_LevelID;
        Fehlversuche = fehlversuche;
    }

    public int Gruppe_GruppeID { get; set; }
    [ForeignKey("Gruppe_GruppeID")]
    public virtual Gruppe Gruppe { get; set; }


    [ForeignKey("Level_LevelID")]
    public int Level_LevelID { get; set; }
    public virtual Level Level { get; set; }

    public TimeSpan BenoetigteZeit { get; set; }
    public int Fehlversuche { get; set; }
}


public class GruppeAbsolviertLevelConfiguration : IEntityTypeConfiguration<GruppeAbsolviertLevel>
{
    public void Configure(EntityTypeBuilder<GruppeAbsolviertLevel> entity)
    {
        entity.ToTable("vcc_gruppe_absolviert_level");
        entity.HasKey(gcl => new { gcl.Gruppe_GruppeID, gcl.Level_LevelID });

        entity.HasOne(gcl => gcl.Gruppe)
              .WithMany(g => g.Absolviert)
              .HasForeignKey(gcl => gcl.Gruppe_GruppeID)
              .HasPrincipalKey(g => g.GruppenID);

        entity.HasOne(gcl => gcl.Level)
              .WithMany(g => g.Absolviert)
              .HasForeignKey(gcl => gcl.Level_LevelID)
              .HasPrincipalKey(l => l.LevelID);
        

    }
}




