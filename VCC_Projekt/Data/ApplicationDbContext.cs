using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VCC_Projekt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(20);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }


        // DbSets for the tables
        public DbSet<Gruppe> Groups { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<GruppeAbsolviertLevel> GruppeAbsolviertLevels { get; set; }
        public DbSet<Aufgabe> Aufgabe { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }

    }




}
