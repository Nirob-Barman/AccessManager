using CleanArchitecture.Application.Common.Models.Authorization;

namespace CleanArchitecture.Infrastructure.Authorization
{

    public static class MenuConfiguration
    {
        public static List<MenuItem> GetMenus()
        {
            return new List<MenuItem>
            {
                new MenuItem
                {
                    Key = "Users",
                    Name = "User Management",
                    Icon = "bi bi-people",
                    SortOrder = 1,

                    SubMenus = new List<SubMenuItem>
                    {
                        new SubMenuItem
                        {
                            Key = "List",
                            Name = "User List",
                            Controller = "User",
                            Action = "Index",
                            SortOrder = 1,

                            Permissions = new List<string>
                            {
                                "View",
                                "Create",
                                "Edit",
                                "Delete",
                                "ResetPassword",
                                "Activate",
                                "Deactivate",
                                "Export"
                            }
                        },

                        new SubMenuItem
                        {
                            Key = "Roles",
                            Name = "Role Management",
                            Controller = "RoleManagement",
                            Action = "Index",
                            SortOrder = 2,

                            Permissions = new List<string>
                            {
                                "View",
                                "Create",
                                "Edit",
                                "Delete",
                                "AssignRole",
                                "RemoveRole"
                            }
                        }
                    }
                },

                new MenuItem
                {
                    Key = "Products",
                    Name = "Product Management",
                    Icon = "bi bi-box",
                    SortOrder = 2,

                    SubMenus = new List<SubMenuItem>
                    {
                        new SubMenuItem
                        {
                            Key = "List",
                            Name = "Product List",
                            Controller = "Product",
                            Action = "Index",
                            SortOrder = 1,

                            Permissions = new List<string>
                            {
                                "View",
                                "Create",
                                "Edit",
                                "Delete",
                                "Approve",
                                "Reject",
                                "Export"
                            }
                        }
                    }
                },

                new MenuItem
                {
                    Key = "Reports",
                    Name = "Reports",
                    Icon = "bi bi-bar-chart",
                    SortOrder = 3,

                    SubMenus = new List<SubMenuItem>
                    {
                        new SubMenuItem
                        {
                            Key = "Sales",
                            Name = "Sales Reports",
                            Controller = "Report",
                            Action = "Sales",
                            SortOrder = 1,

                            Permissions = new List<string>
                            {
                                "View",
                                "ExportPdf",
                                "ExportExcel",
                                "Print"
                            }
                        }
                    }
                }
            };
        }
    }
}