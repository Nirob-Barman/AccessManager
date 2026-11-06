namespace CleanArchitecture.Web.Models.SuperAdminDashboard
{
    public class SuperAdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalPermissions { get; set; }

        public List<RoleStatisticViewModel> RoleStatistics { get; set; } = new();

        public List<RecentUserViewModel> RecentUsers { get; set; } = new();

        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
    }

    public class RoleStatisticViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }

    public class RecentUserViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RecentActivityViewModel
    {
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
