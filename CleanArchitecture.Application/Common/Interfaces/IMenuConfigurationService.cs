using CleanArchitecture.Application.Common.Models.Authorization;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IMenuConfigurationService
    {
        List<MenuItem> GetMenus();
    }
}