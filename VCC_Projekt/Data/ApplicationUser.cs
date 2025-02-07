using System.ComponentModel.DataAnnotations.Schema;

namespace VCC_Projekt.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUser : IdentityUser<string>
{
    public ApplicationUser()
    {
    }

    public ApplicationUser(string firstname, string lastname, int? gruppe_GruppenID)
    {
        Firstname = firstname;
        Lastname = lastname;
        Gruppe_GruppenID = gruppe_GruppenID;
    }

    // Der Benutzername wird als ID verwendet
    public override string Id
    {
        get => UserName;
        set => UserName = value;
    }
    public string Firstname { get; set; }

    public string Lastname { get; set; }
    public int? Gruppe_GruppenID { get; set; }

    public List<Gruppe>? GruppenleiterNavigation { get; set; }

    [ForeignKey("Gruppe_GruppenID")]
    public virtual Gruppe Gruppe { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("vcc_AspNetUsers");
        builder.HasKey(u => u.UserName);
        builder.Property(u => u.UserName)
                  .IsRequired()
                  .HasMaxLength(256);
        builder.HasOne(g => g.Gruppe)
            .WithMany()
            .HasForeignKey(g => g.Gruppe_GruppenID)
            .HasPrincipalKey(k => k.GruppenID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.NormalizedUserName)
              .HasMaxLength(256);
        builder.Ignore(e => e.LockoutEnabled);
        builder.Ignore(e => e.LockoutEnd);
        builder.Ignore(e => e.PhoneNumber);
        builder.Ignore(e => e.PhoneNumberConfirmed);
        builder.Ignore(e => e.TwoFactorEnabled);
        builder.Ignore(e => e.Id);
    }
}