using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace VCC_Projekt.Components.Pages
{
    public partial class EditRoles
    {
        // For MudAutocomplete Searchbox
        private InputModel modelUser = new(null, null);
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
            users = dbContext.Users
                    .GroupJoin(
                        dbContext.UserRoles,
                        u => u.UserName,
                        ur => ur.UserId,
                        (u, userRoles) => new { u, userRoles }
                    )
                    .SelectMany(
                        x => x.userRoles.DefaultIfEmpty(),
                        (x, ur) => new User
                        (
                            x.u.UserName,
                            x.u.Firstname,
                            x.u.Lastname,
                            x.u.Email,
                            ur != null ? ur.RoleId : "Gesperrt"
                        )
                    )
                    .AsNoTracking()
                    .ToList();
            roles = dbContext.Roles.Select(r => r.Name).ToList();
            roles.Add("Gesperrt");
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
            if (modelUser.Role == "Gesperrt")
            {
                var roles = await Usermanager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    await Usermanager.RemoveFromRoleAsync(user, role);
                }
                successMessage = $"User {modelUser.User.Email} wurde gesperrt.";
                showSuccessAlert = true;
            }
            else
            {
                if (!await Usermanager.IsInRoleAsync(user, modelUser.Role))
                {
                    var result = await Usermanager.AddToRoleAsync(user, modelUser.Role);
                    if (result.Succeeded)
                    {
                        successMessage = "Rolle wurde hinzugefügt";
                        showSuccessAlert = true;
                    }
                    else
                    {
                        errorMessage = "Fehler beim hinzufügen der Rolle";
                        showErrorAlert = true;
                    }
                }
                else
                {
                    errorMessage = "Rolle für Benutzer existiert schon!";
                    showErrorAlert = true;
                }
            }
            OnInitialized();
            StateHasChanged();
        }

        private async Task RemoveRole(User user)
        {
            showErrorAlert = false;
            showSuccessAlert = false;
            var userResult = await Usermanager.FindByEmailAsync(user.Email);
            if (user.Rolename == "Gesperrt")
            {
                await Usermanager.AddToRoleAsync(userResult, "Benutzer");
                showSuccessAlert = true;
                successMessage = $"Der gesperrte User {user.Email} wurde wieder als Benutzer angelegt.";
                return;
            }

            await Usermanager.RemoveFromRoleAsync(userResult, user.Rolename);
            showSuccessAlert = true;
            successMessage = $"Rolle wurde entzogen ({user.ToString()})";
            OnInitialized();
            StateHasChanged();
        }

        private Func<User, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return false;
            _searchString = _searchString.ToLower();

            if ($"{x.Email} {x.Fullname} {x.Username} {x.Rolename}".ToLower().Contains(_searchString))
                return true;

            return false;
        };


        private void CloseMe(string name)
        {
            if (name == "Error")
            {
                showErrorAlert = false;
            }
            else if (name == "Success")
            {
                showSuccessAlert = false;
            }
        }
    }


    public record class User(string Username, string Firstname, string Lastname, string Email, string? Rolename)
    {
        public string Fullname => $"{Firstname} {Lastname}";
        public override string ToString()
        {
            return $"{Username} ({Email})";
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "User muss ausgewählt sein")]
        public User? User { get; set; }

        [Required(ErrorMessage = "Rolle mus ausgewählt sein")]
        public string? Role { get; set; }

        // Konstruktor zum Setzen der Werte
        public InputModel(User user, string role)
        {
            User = user;
            Role = role;
        }
    }

}
