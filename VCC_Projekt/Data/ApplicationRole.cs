using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

public class ApplicationRole : IdentityRole
{
    public override string Id { get => base.Name; set => base.Name = value; }
    public new string Beschreibung { get; set; }
}

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("vcc_aspnetroles");
        builder.HasKey(r => r.Name);
        builder.Property(r => r.NormalizedName)
               .HasMaxLength(256);
    }
}