using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace VCC_Projekt.Data;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    void IEntityTypeConfiguration<IdentityUserRole<string>>.Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.ToTable("vcc_AspNetUserRoles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne<ApplicationUser>()
              .WithMany()
              .HasForeignKey(ur => ur.UserId)
              .HasPrincipalKey("UserName")
              .IsRequired();

        builder.HasOne<ApplicationRole>()
              .WithMany()
              .HasForeignKey(ur => ur.RoleId)
              .HasPrincipalKey("Name")
              .IsRequired();
    }
}

public class UserClaimsConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("vcc_AspNetUserClaims")
               .HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .HasPrincipalKey(u => u.UserName);
    }
}

public class RoleClaimsConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("vcc_AspNetRoleClaims")
               .HasOne<ApplicationRole>()
               .WithMany()
               .HasForeignKey(rc => rc.RoleId)
               .HasPrincipalKey(r => r.Name);
    }
}

public class UserLoginsConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("vcc_AspNetUserLogins")
               .HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(l => l.UserId)
               .HasPrincipalKey(u => u.UserName);
    }
}

public class UserTokensConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        builder.ToTable("vcc_AspNetUserTokens")
               .HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .HasPrincipalKey(u => u.UserName);
    }
}
