using System.ComponentModel.DataAnnotations.Schema;

namespace VCC_Projekt.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<string>
    {
        // Der Benutzername wird als ID verwendet
        public override string Id
        {
            get => UserName;
            set => UserName = value;
        }
        // Der Vorname des Benutzers
        public string? Firstname { get; set; }

        // Der Nachname des Benutzers
        public string? Lastname { get; set; }
        public int? Gruppe_GruppenID { get; set; }

        
        public ICollection<Gruppe> GruppenleiterNavigation { get; set; }

        [ForeignKey("Gruppe_GruppenID")]
        public virtual Gruppe Gruppe { get; set; }
    }

}