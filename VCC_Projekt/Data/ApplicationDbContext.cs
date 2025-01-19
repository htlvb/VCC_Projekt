using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VCC_Projekt.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabelle vcc_AspNetUsers konfigurieren
            var user = modelBuilder.Entity<ApplicationUser>()
                .ToTable("vcc_AspNetUsers");
            user.HasKey(u => u.UserName);
            user.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(256);

            user.Property(u => u.NormalizedUserName)
                  .HasMaxLength(256);
            user.Ignore(e => e.LockoutEnabled);
            user.Ignore(e => e.LockoutEnd);
            user.Ignore(e => e.PhoneNumber);
            user.Ignore(e => e.PhoneNumberConfirmed);
            user.Ignore(e => e.TwoFactorEnabled);
            user.Ignore(e => e.Id);


            // Tabelle vcc_AspNetRoles konfigurieren
            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("vcc_AspNetRoles");

                entity.HasKey(r => r.Name);

                entity.Property(r => r.NormalizedName)
                      .HasMaxLength(256);
                entity.Property(e => e.Id).HasColumnName("Name");
                entity.Ignore(r => r.Id);
            });

            // Tabelle vcc_AspNetUserRoles konfigurieren
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserRoles");

                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(ur => ur.UserId)
                      .HasPrincipalKey(a => a.UserName)
                      .IsRequired();

                entity.HasOne<ApplicationRole>()
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId)
                      .HasPrincipalKey(a => a.Name)
                      .IsRequired();

            });

            // Ignoriere ungenutzte Identity-Tabellen
            // Verknüpfe die Claims-Tabellen korrekt
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .ToTable("vcc_AspNetUserClaims")
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasPrincipalKey(u => u.UserName); // Verknüpft UserId mit UserName

            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .ToTable("vcc_AspNetRoleClaims")
                .HasOne<ApplicationRole>()
                .WithMany()
                .HasForeignKey(rc => rc.RoleId)
                .HasPrincipalKey(r => r.Name); // Verknüpft RoleId mit Name

            // Verknüpfe UserLogins mit UserName als ID
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .ToTable("vcc_AspNetUserLogins")
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .HasPrincipalKey(u => u.UserName); // Verknüpft UserId mit UserName

            // Verknüpfe UserTokens mit UserName als ID
            modelBuilder.Entity<IdentityUserToken<string>>()
                .ToTable("vcc_AspNetUserTokens")
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .HasPrincipalKey(u => u.UserName); // Verknüpft UserId mit UserName
        }
    }
}





