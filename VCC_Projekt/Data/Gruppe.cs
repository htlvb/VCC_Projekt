namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Add profile data for application users by adding properties to the ApplicationUser class
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// Entität für die Tabelle _gruppe
public class Gruppe
{
    public Gruppe() { }
    public Gruppe(string gruppenname, int event_EventID, string gruppenleiterId, string teilnehmertyp)
    {
        Gruppenname = gruppenname;
        Event_EventID = event_EventID;
        GruppenleiterId = gruppenleiterId;
        Teilnehmertyp = teilnehmertyp;
    }

    public int GruppenID { get; set; }

    public string? Gruppenname { get; set; }

    // Fremdschlüssel für Event
    public int Event_EventID { get; set; }

    // Fremdschlüssel für Gruppenleiter
    public string GruppenleiterId { get; set; }  // Typ für IdentityUser (string)

    // Navigation Properties

    [ForeignKey("GruppenleiterId")]
    public virtual ApplicationUser GruppenleiterNavigation { get; set; }

    [ForeignKey("Event_EventID")]
    public virtual Event Event { get; set; }  // Navigation zu Event

    public bool Gesperrt { get; set; }

    public string Teilnehmertyp { get; set; }

    // Beziehung zu 'GruppeAbsolviertLevel' 
    public List<GruppeAbsolviertLevel>? Absolviert { get; set; }

    public List<UserInGruppe>? UserInGruppe { get; set; }
}

public class GruppeConfiguration : IEntityTypeConfiguration<Gruppe>
{
    public void Configure(EntityTypeBuilder<Gruppe> builder)
    {
        builder.ToTable("vcc_gruppe");
        builder.HasKey(g => g.GruppenID);

        builder.Property(g => g.Gruppenname)
              .HasMaxLength(255);

        builder.HasOne(g => g.Event)
                              .WithMany(e => e.Gruppen)
                              .HasForeignKey(g => g.Event_EventID)
                              .HasPrincipalKey(e => e.EventID);

        builder.HasMany(g => g.UserInGruppe)
               .WithOne(ug => ug.Gruppe)
               .HasForeignKey(ug => ug.Gruppe_GruppenId)
               .HasPrincipalKey(g => g.GruppenID)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.GruppenleiterNavigation)
               .WithMany(u => u.GruppenleiterNavigation)
               .HasForeignKey(g => g.GruppenleiterId)
               .HasPrincipalKey(u => u.UserName);

        builder.HasMany(g => g.Absolviert)
               .WithOne(gal => gal.Gruppe)
               .HasForeignKey(gal => gal.Gruppe_GruppeID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
