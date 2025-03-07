namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations.Schema;

public class EingeladeneUserInGruppe
{

    public EingeladeneUserInGruppe()
    {
    }

    public EingeladeneUserInGruppe(string email, int gruppenId)
    {
        Email = email;
        Gruppe_GruppenId = gruppenId;
    }

    public string Email { get; set; }

    public int Gruppe_GruppenId { get; set; }

    [ForeignKey("Gruppe_GruppenId")]
    public virtual Gruppe Gruppe { get; set; }
}

public class EingeladeneUserInGruppeConfiguration : IEntityTypeConfiguration<EingeladeneUserInGruppe>
{
    public void Configure(EntityTypeBuilder<EingeladeneUserInGruppe> builder)
    {
        builder.ToTable("vcc_EingeladeneUserInGruppe");

        // Composite Primary Key für UserId und GruppenId
        builder.HasKey(t => new { t.Gruppe_GruppenId });

        // Beziehung zu Gruppe
        builder.HasOne(t => t.Gruppe)
               .WithMany(t => t.EingeladeneUserInGruppe)
               .HasForeignKey(t => t.Gruppe_GruppenId)
               .HasPrincipalKey(t => t.GruppenID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
