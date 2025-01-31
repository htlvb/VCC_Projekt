namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Add profile data for application users by adding properties to the ApplicationUser class
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// Entität für die Tabelle _level
public class Level
{
    public Level() { }
    public Level(byte[] angabe_PDF, int event_EventID)
    {
        Angabe_PDF = angabe_PDF;
        Event_EventID = event_EventID;
    }

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

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> entity)
    {
        entity.ToTable("vcc_level");
        entity.HasKey(l => l.LevelID);
        entity.HasOne(l => l.Event)
              .WithMany(e => e.Levels)
              .HasForeignKey(l => l.Event_EventID)
              .HasPrincipalKey(e => e.EventID);
    }
}