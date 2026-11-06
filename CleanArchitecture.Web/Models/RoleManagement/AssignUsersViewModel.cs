namespace CleanArchitecture.Web.Models.RoleManagement
{
    public class AssignUsersViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

        public List<UserRoleItemViewModel> Users { get; set; }
            = new();
    }
}
