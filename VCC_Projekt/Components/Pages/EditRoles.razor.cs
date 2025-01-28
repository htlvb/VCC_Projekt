using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class EditRoles
    {
        // For MudAutocomplete Searchbox
        private InputModel modelUser = new(null,null);
        private List<string> roles;

        private string _searchString;
        bool showSuccessAlert = false;
        bool showErrorAlert = false;
        string errorMessage = "";
        string successMessage = "";

        // For the Grid
        private List<User> users;
        

        protected override void OnInitialized()
        {
            users = dbContext.UserRoles.AsNoTracking()
                .Where(ur => ur.RoleId == "Admin" || ur.RoleId == "Editor")
                .Join(
                dbContext.Users,
                ur => ur.UserId,
                u => u.Id,
                (ur, u) => new User
                (
                    u.UserName,
                    u.Firstname,
                    u.Lastname,
                    u.Email,
                    ur.RoleId
                )
                )
                .ToList();
            roles = dbContext.Roles.Where(r => r.Name != "Benutzer").Select(r => r.Name).ToList();
        }

        private async Task<IEnumerable<User>> SearchUsers(string searchText, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(searchText)) return Enumerable.Empty<User>(); 

            string tempSearchtext = searchText.ToUpper();

            return await dbContext.Users
                        .AsNoTracking()
                        .Where(u => u.NormalizedEmail.Contains(tempSearchtext) ||
                                    u.NormalizedUserName.Contains(tempSearchtext) ||
                                    u.Firstname.ToUpper().Contains(tempSearchtext) ||
                                    u.Lastname.ToUpper().Contains(tempSearchtext))
                        .Select(u => new User(u.UserName, u.Firstname, u.Lastname, u.Email, null))
                        .ToListAsync(cancellationToken);
        }

        private async Task AddRole()
        {
            showErrorAlert = false;
            showSuccessAlert = false;
            var user = await Usermanager.FindByEmailAsync(modelUser.User.Email);
            if (!await Usermanager.IsInRoleAsync(user, modelUser.Role))
            {
                var result = await Usermanager.AddToRoleAsync(user, modelUser.Role);
                if (result.Succeeded)
                {
                    showSuccessAlert = true;
                    successMessage = "Rolle wurde hinzugefügt";
                }
                else
                {
                    showErrorAlert = true;
                    errorMessage = "Fehler beim hinzufügen der Rolle";
                }
            }
            else
            {
                showErrorAlert = true;
                errorMessage = "Rolle für Benutzer existiert schon!";
            }
            OnInitialized();
            StateHasChanged();

        }

        private async Task RemoveRole(User user)
        {
            var userResult = await Usermanager.FindByEmailAsync(user.Email);
            await Usermanager.RemoveFromRoleAsync(userResult,user.Rolename);
            showSuccessAlert = true ;
            successMessage = $"Rolle wurde entzogen ({user.ToString()})";
            OnInitialized();
            StateHasChanged();
        }

        private Func<User, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if ($"{x.Email} {x.Fullname} {x.Username} {x.Rolename}".Contains(_searchString))
                return true;

            return false;
        };


        private void CloseMe(string name)
        {
            if (name == "Error")
            {
                showErrorAlert = false;
            }
            else if(name == "Success")
            {
                showSuccessAlert = false;
            }
        }
    }


    public record class User(string Username,string Firstname, string Lastname, string Email, string? Rolename)
    {
        public string Fullname => $"{Firstname} {Lastname}";
        public override string ToString()
        {
            return $"{Username} ({Email})";
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage ="User muss ausgewählt sein")]
        public User? User { get; set; }

        [Required(ErrorMessage ="Rolle mus ausgewählt sein")]
        public string? Role { get; set; }

        // Konstruktor zum Setzen der Werte
        public InputModel(User user, string role)
        {
            User = user;
            Role = role;
        }
    }

}
