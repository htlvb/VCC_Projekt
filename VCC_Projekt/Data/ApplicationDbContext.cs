using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VCC_Projekt.Data
{
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
            user.HasOne(g => g.Gruppe)
                .WithMany()
                .HasForeignKey(g => g.Gruppe_GruppenID)
                .HasPrincipalKey(k => k.GruppenID)
                .OnDelete(DeleteBehavior.Restrict);

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
            });

            // Tabelle vcc_AspNetUserRoles konfigurieren
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("vcc_AspNetUserRoles");

                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(ur => ur.UserId)
                      .HasPrincipalKey("UserName")
                      .IsRequired();

                entity.HasOne<ApplicationRole>()
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId)
                      .HasPrincipalKey("Name")
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

            

            // Tabelle vcc_gruppe konfigurieren
            modelBuilder.Entity<Gruppe>(entity =>
            {
                entity.ToTable("vcc_gruppe");
                entity.HasKey(g => g.GruppenID);

                entity.Property(g => g.Gruppenname)
                      .HasMaxLength(255);

                // Korrekte Beziehung zu Event
                entity.HasOne(g => g.Event)
                      .WithMany(e => e.Gruppen)  // Ein Event kann viele Gruppen haben
                      .HasForeignKey(g => g.Event_EventID)
                      .HasPrincipalKey(e => e.EventID);

                // Korrekte Beziehung zum Gruppenleiter
                entity.HasOne(g => g.GruppenleiterNavigation)
                      .WithMany()
                      .HasForeignKey(g => g.GruppenleiterId)
                      .HasPrincipalKey(u => u.UserName);
            });

            // Tabelle vcc_level konfigurieren
            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("vcc_level");
                entity.HasKey(l => l.LevelID);

                // Korrekte Beziehung zu Event
                entity.HasOne(l => l.Event)
                      .WithMany(e => e.Levels)  // Ein Event kann viele Levels haben
                      .HasForeignKey(l => l.Event_EventID)
                      .HasPrincipalKey(e => e.EventID);
            });

            // Tabelle vcc_event konfigurieren
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("vcc_event");
                entity.HasKey(e => e.EventID);
            });

            // Weitere Konfigurationen für vcc_gruppe_absolviert_level
            modelBuilder.Entity<GruppeAbsolviertLevel>(entity =>
            {
                entity.ToTable("vcc_gruppe_absolviert_level");
                entity.HasKey(gcl => new { gcl.Gruppe_GruppeID, gcl.Level_LevelID });

                entity.HasOne(gcl => gcl.Gruppe)
                      .WithMany()
                      .HasForeignKey(gcl => gcl.Gruppe_GruppeID)
                      .HasPrincipalKey(g => g.GruppenID);

                entity.HasOne(gcl => gcl.Level)
                      .WithMany()
                      .HasForeignKey(gcl => gcl.Level_LevelID)
                      .HasPrincipalKey(l => l.LevelID);
            });

            // Tabelle vcc_aufgaben konfigurieren
            modelBuilder.Entity<Aufgabe>(entity =>
            {
                entity.ToTable("vcc_aufgaben");
                entity.HasKey(t => t.AufgabenID);

                // Korrekte Beziehung zu Level
                entity.HasOne(t => t.Level)
                      .WithMany()
                      .HasForeignKey(t => t.Level_LevelID)
                      .HasPrincipalKey(l => l.LevelID);
            });
        }


        // DbSets for the tables
        public DbSet<Gruppe> Groups { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<GruppeAbsolviertLevel> GruppeAbsolviertLevels { get; set; }
        public DbSet<Aufgabe> Aufgabe { get; set; }
    }

    

    
}
