using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VCC_Projekt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<int>, int>
    {
        // Konstruktor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Konfiguriere das Modell (zum Beispiel Tabellennamen ändern)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabelle vcc_AspNetUsers konfigurieren
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("vcc_AspNetUsers");

                // Optional: Entfernen nicht benötigter Felder
                entity.Ignore(c => c.TwoFactorEnabled);
                entity.Ignore(c => c.LockoutEnabled);
                entity.Ignore(c => c.LockoutEnd);
                entity.Ignore(c => c.PhoneNumber);
                entity.Ignore(c => c.PhoneNumberConfirmed);
            });

            // Tabelle vcc_AspNetRoles konfigurieren
            modelBuilder.Entity<IdentityRole<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetRoles");
            });

            // Tabelle vcc_AspNetUserRoles konfigurieren
            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserRoles");
            });

            // Tabelle vcc_AspNetUserClaims konfigurieren
            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserClaims");
            });

            // Tabelle vcc_AspNetUserLogins konfigurieren
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserLogins");
            });

            // Tabelle vcc_AspNetRoleClaims konfigurieren
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetRoleClaims");
            });

            // Tabelle vcc_AspNetUserTokens konfigurieren
            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserTokens");
            });
        }

    }
}
