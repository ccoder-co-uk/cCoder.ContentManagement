using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IPageRoleService
{
    IQueryable<PageRole> GetAll(bool ignoreFilters = false);

    ValueTask<PageRole> AddAsync(PageRole pageRole);

    ValueTask DeleteAsync(PageRole pageRole);
}
