using Microsoft.AspNetCore.Components;
using static VCC_Projekt.Components.Pages.EditRoles;

namespace VCC_Projekt.Components.Pages
{
    public partial class EditRoles
    {
        // For MudAutocomplete Searchbox
        private User selectedUser;
        private Role selectedRole;

        // For the Grid
        private List<User> users;

        protected override void OnInitialized()
        {
            return;
        }

        private async Task<IEnumerable<User>> SearchUsers(string searchText, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<Role>> SearchRoles(string searchText, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void AddRole()
        {
            throw new NotImplementedException();
        }

        private void RemoveRole(User user)
        {
            throw new NotImplementedException();
        }

        public record class User(string Username, string Email, Role role);
        public record class Role(string RoleName, string Description);
    }
}
