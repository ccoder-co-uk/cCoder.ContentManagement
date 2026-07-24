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

internal partial class PageRoleProcessingService(
    IPageRoleService service,
    IPageRoleBroker pageRoleBroker,
    IRoleBroker roleBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IPageRoleProcessingService
{
    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public IQueryable<PageRole> GetAllPageRole(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageRole>>(operation: () =>
    {
        ValidateAllPageRoleOnGet(inputs: [ignoreFilters]);
        return service.GetAllPageRole(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole) =>
        TryCatch<PageRole>(operation: () =>
    {
        ValidatePageRoleOnAdd(inputs: [newPageRole]);
        ValidatePageRole(pageRole: newPageRole, parameterName: "entity");
        var (role, page) = GetRoleAndPage(entity: newPageRole);

        if (role != null && page != null && ContentManagementModelLogic.UserCan(page: page, user: GetCurrentUser(), privilege: "pagerole_create"))
        {
            return (!(page.Roles ?? Array.Empty<PageRole>()).Any(predicate: (PageRole r) => r.RoleId == role.Id))
                ? service.AddPageRoleAsync(newPageRole: newPageRole)
                : ValueTask.FromResult(result: newPageRole);
        }

        throw new SecurityException(message: "Access Denied!");

    }, isValueTask: true);

    public ValueTask DeletePageRoleAsync(PageRole deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidatePageRoleOnDelete(inputs: [deletedPageRole]);
        ValidatePageRole(pageRole: deletedPageRole, parameterName: "link");

        Page page = pageBroker.GetAllPages(ignoreFilters: true)
            .FirstOrDefault(predicate: existingPage => existingPage.Id == deletedPageRole.PageId);

        PageRole dbVersion = service.GetAllPageRole(ignoreFilters: true)
            .FirstOrDefault(predicate: pageRole => pageRole.RoleId == deletedPageRole.RoleId && pageRole.PageId == deletedPageRole.PageId);

        if (dbVersion == null || page == null || !ContentManagementModelLogic.UserCan(page: page, user: GetCurrentUser(), privilege: "pagerole_delete"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await service.DeletePageRoleAsync(deletedPageRole: dbVersion);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PageRole>>> AddOrUpdatePageRoleResult(IEnumerable<PageRole> newPageRole) =>
        TryCatch<IEnumerable<OperationResult<PageRole>>>(operation: async () =>
    {
        ValidateOrUpdatePageRoleResultOnAdd(inputs: [newPageRole]);
        ValidatePageRoles(pageRoles: newPageRole, parameterName: "items");
        PageRole[] itemArray = newPageRole.ToArray();

        int[] leftIds = itemArray
            .Select(selector: item => item.PageId)
            .Distinct()
            .ToArray();

        PageRole[] existingItems = ExecuteGetAllPageRole()
            .Where(predicate: item => ((ReadOnlySpan<int>)leftIds).Contains(value: item.PageId))
            .ToArray();

        List<OperationResult<PageRole>> results = new List<OperationResult<PageRole>>();

        foreach (IGrouping<int, PageRole> group in itemArray.GroupBy(keySelector: item => item.PageId))
        {
            PageRole[] groupItems = group.ToArray();

            PageRole[] existingGroupItems = existingItems
                .Where(predicate: item => object.Equals(objA: item.PageId, objB: group.Key))
                .ToArray();

            await ExecuteDeleteAllPageRoleAsync(deletedPageRole: existingGroupItems);

            foreach (PageRole item in groupItems)
            {
                try
                {
                    results.Add(item: new OperationResult<PageRole>
                    {
                        Id = $"{item.PageId}:{item.RoleId}",
                        Success = true,
                        Item = await ExecuteAddPageRoleAsync(newPageRole: item),
                        Message = "Added Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new OperationResult<PageRole>
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

    }, isValueTask: true);

    public ValueTask ImportPageRoleInfosAsync(int appId, PageRoleInfo[] items) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPageRoleInfosAsync(inputs: [appId, items]);
        ValidateAppId(appId: appId, parameterName: "appId");
        ValidatePageRoleInfos(pageRoleInfos: items, parameterName: "items");

        Role[] roles = roleBroker.GetAllRoles(ignoreFilters: true)
            .Where(predicate: role => role.AppId == appId)
            .ToArray();

        Page[] pages = pageBroker.GetAllPages(ignoreFilters: true)
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
            await pageRoleBroker.DeleteAllPageRolesAsync(deletedPageRole: pageRolesToDelete);
        }

        foreach (PageRole pageRole in pageRoles
            .Where(predicate: incoming => !existingPageRoles.Any(predicate: existing =>
                existing.PageId == incoming.PageId
                && existing.RoleId == incoming.RoleId)))
        {
            await pageRoleBroker.AddPageRoleAsync(newPageRole: pageRole);
        }

    }, isValueTask: true);

    public ValueTask DeleteAllPageRoleAsync(IEnumerable<PageRole> deletedPageRole) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageRoleOnDelete(inputs: [deletedPageRole]);
        ValidatePageRoles(pageRoles: deletedPageRole, parameterName: "items");

        foreach (PageRole item in deletedPageRole)
        {
            await ExecuteDeletePageRoleAsync(deletedPageRole: item);
        }

    }, isValueTask: true);

    private (Role role, Page page) GetRoleAndPage(PageRole entity) =>
        (
            role: roleBroker.GetAllRoles(ignoreFilters: true)
        .Where(predicate: role => role.Id == entity.RoleId)
        .FirstOrDefault(),
            page: pageBroker.GetAllPages(ignoreFilters: true)
        .FirstOrDefault(predicate: page => page.Id == entity.PageId));

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

    private ValueTask<PageRole> ExecuteAddPageRoleAsync(PageRole newPageRole)
    {
        ValidatePageRole(pageRole: newPageRole, parameterName: "entity");
        var (role, page) = GetRoleAndPage(entity: newPageRole);

        if (role != null && page != null && ContentManagementModelLogic.UserCan(page: page, user: GetCurrentUser(), privilege: "pagerole_create"))
        {
            return (!(page.Roles ?? Array.Empty<PageRole>()).Any(predicate: (PageRole r) => r.RoleId == role.Id))
                ? service.AddPageRoleAsync(newPageRole: newPageRole)
                : ValueTask.FromResult(result: newPageRole);
        }

        throw new SecurityException(message: "Access Denied!");
    }

    private async ValueTask ExecuteDeleteAllPageRoleAsync(IEnumerable<PageRole> deletedPageRole)
    {
        ValidatePageRoles(pageRoles: deletedPageRole, parameterName: "items");

        foreach (PageRole item in deletedPageRole)
        {
            await ExecuteDeletePageRoleAsync(deletedPageRole: item);
        }
    }

    private async ValueTask ExecuteDeletePageRoleAsync(PageRole deletedPageRole)
    {
        ValidatePageRole(pageRole: deletedPageRole, parameterName: "link");

        Page page = pageBroker.GetAllPages(ignoreFilters: true)
            .FirstOrDefault(predicate: existingPage => existingPage.Id == deletedPageRole.PageId);

        PageRole dbVersion = service.GetAllPageRole(ignoreFilters: true)
            .FirstOrDefault(predicate: pageRole => pageRole.RoleId == deletedPageRole.RoleId && pageRole.PageId == deletedPageRole.PageId);

        if (dbVersion == null || page == null || !ContentManagementModelLogic.UserCan(page: page, user: GetCurrentUser(), privilege: "pagerole_delete"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await service.DeletePageRoleAsync(deletedPageRole: dbVersion);
    }

    private IQueryable<PageRole> ExecuteGetAllPageRole(bool ignoreFilters = false) =>
        service.GetAllPageRole(ignoreFilters: ignoreFilters);
}