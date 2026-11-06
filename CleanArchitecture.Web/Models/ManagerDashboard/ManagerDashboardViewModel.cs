namespace CleanArchitecture.Web.Models.ManagerDashboard
{
    public class ManagerDashboardViewModel
    {
        public int TotalEmployees { get; set; }

        public int ActiveEmployees { get; set; }

        public int InactiveEmployees { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalUsers { get; set; }

        public List<ManagerEmployeeDto> RecentEmployees { get; set; }
            = new();

        public List<ManagerActivityDto> RecentActivities { get; set; }
            = new();
    }

    public class ManagerEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }

    public class ManagerActivityDto
    {
        public string Description { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
