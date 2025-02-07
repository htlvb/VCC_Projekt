namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations.Schema;

public class UserInGruppe
{

    public UserInGruppe()
    {
    }

    public UserInGruppe(string userId, int gruppenId)
    {
        User_UserId = userId;
        Gruppe_GruppenId = gruppenId;
    }

    public string User_UserId { get; set; }

    public int Gruppe_GruppenId { get; set; }

    [ForeignKey("User_UserId")]
    public virtual ApplicationUser User { get; set; }

    [ForeignKey("Gruppe_GruppenId")]
    public virtual Gruppe Gruppe { get; set; }
}

public class UserInGruppeConfiguration : IEntityTypeConfiguration<UserInGruppe>
{
    public void Configure(EntityTypeBuilder<UserInGruppe> builder)
    {
        builder.ToTable("vcc_UserInGruppe");

        // Composite Primary Key für UserId und GruppenId
        builder.HasKey(t => new { t.User_UserId, t.Gruppe_GruppenId });

        // Beziehung zu User
        builder.HasOne(t => t.User)
               .WithMany(t => t.UserInGruppe)
               .HasForeignKey(t => t.User_UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Beziehung zu Gruppe
        builder.HasOne(t => t.Gruppe)
               .WithMany(t => t.UserInGruppe)
               .HasForeignKey(t => t.Gruppe_GruppenId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
