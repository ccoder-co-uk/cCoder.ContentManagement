using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageRoleProcessingService(
    IPageRoleService service,
    IPageRoleBroker pageRoleBroker,
    IRoleBroker roleBroker,
    IPageService pageService,
    IAuthorizationBroker authorizationBroker) : IPageRoleProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public IQueryable<PageRole> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<PageRole> AddAsync(PageRole entity)
    {
        ValidatePageRole(entity, "entity");
        var (role, page) = GetRoleAndPage(entity);
        if (role != null && page != null && ContentManagementModelLogic.UserCan(page, User, "pagerole_create"))
        {
            return (!(page.Roles ?? Array.Empty<PageRole>()).Any((PageRole r) => r.RoleId == role.Id))
                ? service.AddAsync(entity)
                : ValueTask.FromResult(entity);
        }
        throw new SecurityException("Access Denied!");
    }

    public async ValueTask DeleteAsync(PageRole link)
    {
        ValidatePageRole(link, "link");
        Page page = pageService.GetAll(ignoreFilters: true)
            .FirstOrDefault(existingPage => existingPage.Id == link.PageId);
        PageRole dbVersion = service.GetAll(ignoreFilters: true)
            .FirstOrDefault(pageRole => pageRole.RoleId == link.RoleId && pageRole.PageId == link.PageId);
        if (dbVersion == null || page == null || !ContentManagementModelLogic.UserCan(page, User, "pagerole_delete"))
            throw new SecurityException("Access Denied!");

        await service.DeleteAsync(dbVersion);
    }

    public async ValueTask<IEnumerable<Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items)
    {
        ValidatePageRoles(items, "items");
        PageRole[] itemArray = items.ToArray();
        int[] leftIds = itemArray
            .Select(item => item.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingItems = GetAll()
            .Where(item => ((ReadOnlySpan<int>)leftIds).Contains(item.PageId))
            .ToArray();

        List<Result<PageRole>> results = new List<Result<PageRole>>();
        foreach (IGrouping<int, PageRole> group in itemArray.GroupBy(item => item.PageId))
        {
            PageRole[] groupItems = group.ToArray();
            PageRole[] existingGroupItems = existingItems
                .Where(item => object.Equals(item.PageId, group.Key))
                .ToArray();

            await DeleteAllAsync(existingGroupItems);
            foreach (PageRole item in groupItems)
            {
                try
                {
                    results.Add(new Result<PageRole>
                    {
                        Id = $"{item.PageId}:{item.RoleId}",
                        Success = true,
                        Item = await AddAsync(item),
                        Message = "Added Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new Result<PageRole>
                    {
                        Id = $"{item.PageId}:{item.RoleId}",
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }
        }
        return results;
    }

    public async ValueTask ImportPageRolesAsync(int appId, PageRoleInfo[] items)
    {
        ValidateAppId(appId, "appId");
        ValidatePageRoleInfos(items, "items");
        Role[] roles = roleBroker.GetAllRoles(ignoreFilters: true)
            .Where(role => role.AppId == appId)
            .ToArray();

        Page[] pages = pageService.GetAll(ignoreFilters: true)
            .Where(page => page.AppId == appId)
            .ToArray();

        PageRole[] pageRoles = items
            .Select(pageRoleInfo =>
            {
                Page page = pages.FirstOrDefault(existing => existing.Path == pageRoleInfo.Path);
                Role role = roles.FirstOrDefault(existing => existing.Name == pageRoleInfo.Role);

                return new PageRole
                {
                    PageId = (page?.Id ?? 0),
                    RoleId = (role?.Id ?? Guid.Empty)
                };
            })
            .Where(pageRole => pageRole.PageId != 0 && pageRole.RoleId != Guid.Empty)
            .GroupBy(pageRole => new { pageRole.PageId, pageRole.RoleId })
            .Select(group => group.First())
            .ToArray();

        int[] pageIds = pageRoles
            .Select(pageRole => pageRole.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingPageRoles = pageRoleBroker.GetAllPageRoles(ignoreFilters: true)
            .Where(pageRole => ((ReadOnlySpan<int>)pageIds).Contains(pageRole.PageId))
            .ToArray();

        PageRole[] pageRolesToDelete = existingPageRoles
            .Where(existing => !pageRoles.Any(incoming =>
                incoming.PageId == existing.PageId
                && incoming.RoleId == existing.RoleId))
            .ToArray();

        if (pageRolesToDelete.Length > 0)
            await pageRoleBroker.DeleteAllPageRolesAsync(pageRolesToDelete);

        foreach (PageRole pageRole in pageRoles
            .Where(incoming => !existingPageRoles.Any(existing =>
                existing.PageId == incoming.PageId
                && existing.RoleId == incoming.RoleId)))
            await pageRoleBroker.AddPageRoleAsync(pageRole);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PageRole> items)
    {
        ValidatePageRoles(items, "items");
        foreach (PageRole item in items)
            await DeleteAsync(item);
    }

    private (Role role, Page page) GetRoleAndPage(PageRole entity)
    {
        return (
            role: roleBroker.GetAllRoles(ignoreFilters: true)
                .Where(role => role.Id == entity.RoleId)
                .FirstOrDefault(),
            page: pageService.GetAll(ignoreFilters: true)
                .FirstOrDefault(page => page.Id == entity.PageId));
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static void ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
            throw new ValidationException(parameterName + " is required.");

        if (pageRole.PageId < 1)
            throw new ValidationException(parameterName + ".PageId must be greater than 0.");

        if (pageRole.RoleId == Guid.Empty)
            throw new ValidationException(parameterName + ".RoleId is required.");
    }

    private static void ValidatePageRoles(IEnumerable<PageRole> pageRoles, string parameterName)
    {
        if (pageRoles == null)
            throw new ValidationException(parameterName + " is required.");

        foreach (PageRole pageRole in pageRoles)
            ValidatePageRole(pageRole, parameterName);
    }

    private static void ValidatePageRoleInfos(IEnumerable<PageRoleInfo> pageRoleInfos, string parameterName) =>
        ThrowIf(pageRoleInfos == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
