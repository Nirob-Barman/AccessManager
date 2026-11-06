namespace CleanArchitecture.Application.Common.Models.Authorization
{
    public class MenuItem
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public int SortOrder { get; set; }

        public List<SubMenuItem> SubMenus { get; set; } = new();
    }

    public class SubMenuItem
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public int SortOrder { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
    public class SubMenuClaimViewModel
    {
        public string Key { get; set; } = string.Empty;

        public string ParentKey { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";

        public int SortOrder { get; set; }
    }
}
