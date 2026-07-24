// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private User User =>
        authorizationBroker.GetCurrentUser();

    public IQueryable<PageRole> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<PageRole> AddAsync(PageRole entity)
    {
        ValidatePageRole(pageRole: entity, parameterName: "entity");
        var (role, page) = GetRoleAndPage(entity: entity);

        if (role != null && page != null && ContentManagementModelLogic.UserCan(page: page, user: User, privilege: "pagerole_create"))
        {
            return (!(page.Roles ?? Array.Empty<PageRole>()).Any(predicate: (PageRole r) => r.RoleId == role.Id))
                ? service.AddAsync(pageRole: entity)
                : ValueTask.FromResult(result: entity);
        }

        throw new SecurityException(message: "Access Denied!");
    }

    public async ValueTask DeleteAsync(PageRole link)
    {
        ValidatePageRole(pageRole: link, parameterName: "link");

        Page page = pageService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: existingPage => existingPage.Id == link.PageId);

        PageRole dbVersion = service.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: pageRole => pageRole.RoleId == link.RoleId && pageRole.PageId == link.PageId);

        if (dbVersion == null || page == null || !ContentManagementModelLogic.UserCan(page: page, user: User, privilege: "pagerole_delete"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await service.DeleteAsync(pageRole: dbVersion);
    }

    public async ValueTask<IEnumerable<Result<PageRole>>> AddOrUpdate(IEnumerable<PageRole> items)
    {
        ValidatePageRoles(pageRoles: items, parameterName: "items");
        PageRole[] itemArray = items.ToArray();

        int[] leftIds = itemArray
            .Select(selector: item => item.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingItems = GetAll()
            .Where(predicate: item => ((ReadOnlySpan<int>)leftIds).Contains(value: item.PageId))
            .ToArray();

        List<Result<PageRole>> results = new List<Result<PageRole>>();

        foreach (IGrouping<int, PageRole> group in itemArray.GroupBy(keySelector: item => item.PageId))
        {
            PageRole[] groupItems = group.ToArray();

            PageRole[] existingGroupItems = existingItems
                .Where(predicate: item => object.Equals(objA: item.PageId, objB: group.Key))
                .ToArray();

            await DeleteAllAsync(items: existingGroupItems);

            foreach (PageRole item in groupItems)
            {
                try
                {
                    results.Add(item: new Result<PageRole>
                    {
                        Id = $"{item.PageId}:{item.RoleId}",
                        Success = true,
                        Item = await AddAsync(entity: item),
                        Message = "Added Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new Result<PageRole>
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
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePageRoleInfos(pageRoleInfos: items, parameterName: "items");

        Role[] roles = roleBroker.GetAllRoles(ignoreFilters: true)
            .Where(predicate: role => role.AppId == appId)
            .ToArray();

        Page[] pages = pageService.GetAll(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray();

        PageRole[] pageRoles = items
            .Select(selector: pageRoleInfo =>
            {
                Page page = pages.FirstOrDefault(predicate: existing => existing.Path == pageRoleInfo.Path);
                Role role = roles.FirstOrDefault(predicate: existing => existing.Name == pageRoleInfo.Role);

                return new PageRole
                {
                    PageId = (page?.Id ?? 0),
                    RoleId = (role?.Id ?? Guid.Empty)
                };
            })
            .Where(predicate: pageRole => pageRole.PageId != 0 && pageRole.RoleId != Guid.Empty)
            .GroupBy(keySelector: pageRole => new { pageRole.PageId, pageRole.RoleId })
            .Select(selector: group => group.First())
            .ToArray();

        int[] pageIds = pageRoles
            .Select(selector: pageRole => pageRole.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingPageRoles = pageRoleBroker.GetAllPageRoles(ignoreFilters: true)
            .Where(predicate: pageRole => ((ReadOnlySpan<int>)pageIds).Contains(value: pageRole.PageId))
            .ToArray();

        PageRole[] pageRolesToDelete = existingPageRoles
            .Where(predicate: existing => !pageRoles.Any(predicate: incoming =>
                incoming.PageId == existing.PageId
                && incoming.RoleId == existing.RoleId))
            .ToArray();

        if (pageRolesToDelete.Length > 0)
        {
            await pageRoleBroker.DeleteAllPageRolesAsync(items: pageRolesToDelete);
        }

        foreach (PageRole pageRole in pageRoles
            .Where(predicate: incoming => !existingPageRoles.Any(predicate: existing =>
                existing.PageId == incoming.PageId
                && existing.RoleId == incoming.RoleId)))
        {
            await pageRoleBroker.AddPageRoleAsync(entity: pageRole);
        }
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PageRole> items)
    {
        ValidatePageRoles(pageRoles: items, parameterName: "items");

        foreach (PageRole item in items)
        {
            await DeleteAsync(link: item);
        }
    }

    private (Role role, Page page) GetRoleAndPage(PageRole entity)
    {
        return (
            role: roleBroker.GetAllRoles(ignoreFilters: true)
            .Where(predicate: role => role.Id == entity.RoleId)
            .FirstOrDefault(),
            page: pageService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: page => page.Id == entity.PageId));
    }

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (pageRole.PageId < 1)
        {
            throw new ValidationException(message: parameterName + ".PageId must be greater than 0.");
        }

        if (pageRole.RoleId == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + ".RoleId is required.");
        }
    }

    private static void ValidatePageRoles(IEnumerable<PageRole> pageRoles, string parameterName)
    {
        if (pageRoles == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        foreach (PageRole pageRole in pageRoles)
        {
            ValidatePageRole(pageRole: pageRole, parameterName: parameterName);
        }
    }

    private static void ValidatePageRoleInfos(IEnumerable<PageRoleInfo> pageRoleInfos, string parameterName) =>
        ThrowIf(condition: pageRoleInfos == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}