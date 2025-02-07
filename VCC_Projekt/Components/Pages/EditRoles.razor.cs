using MudBlazor;

namespace VCC_Projekt.Components.Pages
{
    public partial class EditRoles
    {
        // For MudAutocomplete Searchbox
        private List<string> roles;

        private string _searchString;

        // For the Grid
        private List<EditRoleUser> users;

        protected override void OnInitialized()
        {
            users = dbContext.Users
                            .GroupJoin(
                                dbContext.UserRoles,
                                u => u.UserName,
                                ur => ur.UserId,
                                (u, userRoles) => new { u, userRoles }
                            )
                            .Select(x => new EditRoleUser
                            (
                                x.u.UserName,
                                x.u.Firstname,
                                x.u.Lastname,
                                x.u.Email,
                                x.userRoles.Any() ? x.userRoles.Select(ur => ur.RoleId).ToList() : new List<string> { "Gesperrt" }
                            ))
                            .AsNoTracking()
                            .ToList();
            roles = dbContext.Roles.Select(r => r.Name).ToList();
            roles.Add("Gesperrt");
        }

        private Func<EditRoleUser, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return false;
            _searchString = _searchString.ToLower();

            if (x.Email.ToLower().StartsWith(_searchString)) return true;
            if (x.Firstname.ToLower().StartsWith(_searchString)) return true;
            if (x.Lastname.ToLower().StartsWith(_searchString)) return true;
            if (x.Username.ToLower().StartsWith(_searchString)) return true;
            if (x.Roles.Any(r => r.ToLower().StartsWith(_searchString))) return true;

            return false;
        };

        private void CommittedItemChanges(EditRoleUser item)
        {
            Task.Run(async () =>
            {
                try
                {
                    ApplicationUser? user = await Usermanager.FindByEmailAsync(item.Email);
                    if (user == null)
                    {
                        Snackbar.Add("Benutzer nicht gefunden, um die Rolle zu ändern!", Severity.Error);
                        await InvokeAsync(StateHasChanged);
                        return;
                    }
                    var currentRoles = await Usermanager.GetRolesAsync(user);
                    var rolesToRemove = currentRoles.Except(item.Roles).ToList();
                    var rolesToAdd = item.Roles.Except(currentRoles).ToList();

                    if (rolesToRemove.Any())
                    {
                        await Usermanager.RemoveFromRolesAsync(user, rolesToRemove.ToArray());
                    }

                    if (rolesToAdd.Any() && !rolesToAdd.Contains("Gesperrt"))
                    {
                        await Usermanager.AddToRolesAsync(user, rolesToAdd.ToArray());
                    }

                    Snackbar.Add($"Rollen erfolgreich geändert! ({item.Email}; {string.Join(",", item.Roles)})", Severity.Success);

                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Fehler bei der Änderung der Rollen: {ex.Message}", Severity.Error);
                    await InvokeAsync(StateHasChanged);
                }
            }).ConfigureAwait(false);
        }

        private void OnRolesChanged(EditRoleUser user, IEnumerable<string> newRoles)
        {
            var updatedRoles = newRoles.ToList();

            if (updatedRoles.Count == 0 && !user.Roles.Contains("Gesperrt"))
            {
                user.Roles = new List<string> { "Gesperrt" };
            }
            else if (user.Roles.Contains("Gesperrt") && !newRoles.Contains("Gesperrt"))
            {
                user.Roles = new List<string> { "Benutzer" };
            }
            else if (newRoles.Contains("Gesperrt"))
            {
                user.Roles = new List<string> { "Gesperrt" };
            }
            else
            {
                user.Roles = updatedRoles;
            }
        }
    }


    public class EditRoleUser
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public EditRoleUser(string username, string firstname, string lastname, string email, List<string> roles)
        {
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Roles = roles;
        }

        public string Fullname => $"{Firstname} {Lastname}";

        public override string ToString()
        {
            return $"{Username} ({Email})";
        }
    }


}
