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

            // Weitere Konfigurationen für vcc_gruppe, vcc_level, etc.
            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("vcc_gruppe");
                entity.HasKey(g => g.GruppenID);

                entity.Property(g => g.Gruppenname)
                      .HasMaxLength(255);

                entity.HasOne<Event>()
                      .WithMany()
                      .HasForeignKey(g => g.EventID);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(g => g.Gruppenleiter)
                      .HasPrincipalKey(u => u.UserName);
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("vcc_level");
                entity.HasKey(l => l.LevelID);

                entity.HasOne<Event>()
                      .WithMany()
                      .HasForeignKey(l => l.EventID);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("vcc_event");
                entity.HasKey(e => e.EventID);
            });

            modelBuilder.Entity<GroupCompletedLevel>(entity =>
            {
                entity.ToTable("vcc_gruppe_absolviert_level");
                entity.HasKey(gcl => new { gcl.GruppeID, gcl.LevelID });

                entity.HasOne<Group>()
                      .WithMany()
                      .HasForeignKey(gcl => gcl.GruppeID);

                entity.HasOne<Level>()
                      .WithMany()
                      .HasForeignKey(gcl => gcl.LevelID);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("vcc_aufgaben");
                entity.HasKey(t => t.AufgabenID);

                entity.HasOne<Level>()
                      .WithMany()
                      .HasForeignKey(t => t.LevelID);
            });
        }

        // DbSets for the tables
        public DbSet<Group> Groups { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<GroupCompletedLevel> GroupCompletedLevels { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }

    

    public class Group
    {
        public int GruppenID { get; set; }
        public string Gruppenname { get; set; }
        public int EventID { get; set; }
        public string Gruppenleiter { get; set; }
    }
}
