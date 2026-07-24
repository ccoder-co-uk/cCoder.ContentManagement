// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService(
    IPageService service,
    IAuthorizationBroker authorizationBroker) : IPageProcessingService
{
    private User User =>
        authorizationBroker.GetCurrentUser();

    public Page Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Page> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public string MenuFor(int id, string culture)
    {
        ValidateId(id: id, parameterName: "id");

        IEnumerable<string> enumerable = service.GetAll(ignoreFilters: false)
            .Where(predicate: page => page.ParentId == id && page.ShowOnMenus)
            .OrderBy(keySelector: page => page.Order)
            .Select(selector: page => $"<li data-id='{page.Id}' class='item'><a href='/{page.Path}'>{ContentManagementModelLogic.Title(page: page, culture: culture)}</a></li>");

        string text = (enumerable.Any() ? string.Join(separator: "", values: enumerable) : string.Empty);
        return "<ul class='submenu'>" + text + "</ul>";
    }

    public Page GetRoot(int id)
    {
        ValidateId(id: id, parameterName: "id");
        Page page = Get(id: id);

        while (page.ParentId.HasValue)
        {
            Page page2 = Get(id: page.ParentId.Value);
            page = page2 ?? page;
        }

        return page;
    }

    public IEnumerable<Page> GetChildren(int id)
    {
        ValidateId(id: id, parameterName: "id");

        return GetAll()
            .Where(predicate: page => page.ParentId == (int?)id);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        if (!UserCan(privKey: "page_delete", pageId: id))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return service.DeleteAsync(id: id);
    }

    public async ValueTask<Page> UpdateAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");

        Page dbVersion = service.GetAll(ignoreFilters: true)
            .Where(predicate: existingPage => existingPage.Id == page.Id)
            .FirstOrDefault();

        if (dbVersion == null || !UserCan(privKey: "page_update", pageId: dbVersion.Id))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Page parent = page.ParentId.HasValue
            ? service.GetAll(ignoreFilters: true)
            .Where(predicate: existingPage => existingPage.Id == page.ParentId.Value)
            .FirstOrDefault()
            : null;

        if (page.ParentId.HasValue && parent == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        dbVersion.ParentId = page.ParentId;
        dbVersion.AppId = page.AppId;
        dbVersion.Order = page.Order;
        dbVersion.ShowOnMenus = page.ShowOnMenus;
        dbVersion.Name = page.Name;
        dbVersion.ResourceKey = page.ResourceKey;
        dbVersion.Layout = page.Layout;
        dbVersion.Path = BuildPath(pageName: page.Name, parentPath: parent?.Path);

        return await service.UpdateAsync(page: dbVersion);
    }

    public async ValueTask<Page> AddAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");

        if (!authorizationBroker.IsAdminOfApp(appId: page.AppId) && page.ParentId.HasValue)
        {
            UserCan(privKey: "page_create", pageId: page.ParentId.Value);
        }

        Page parent = null;

        if (page.ParentId.HasValue)
        {
            parent = service.GetAll(ignoreFilters: false)
                .Where(predicate: existingPage => existingPage.Id == page.ParentId.Value)
                .FirstOrDefault();
        }
        else
        {
            if (page.Path != null && page.Path.Contains(value: '/'))
            {
                string parentPath = GetParentPath(path: page.Path);

                string normalizedParentPath = parentPath.TrimStart(trimChar: '/')
                    .ToLower();

                parent = service.GetAll(ignoreFilters: false)
                    .Where(predicate: existingPage =>
                        existingPage.AppId == page.AppId &&
                        existingPage.Path.ToLower() == normalizedParentPath)
                    .FirstOrDefault();
            }
        }

        page.Path = BuildPath(pageName: page.Name, parentPath: parent?.Path);
        page.ParentId = parent?.Id;
        ValidatePathDoesNotExistForApp(page: page);

        Page newPage = new Page
        {
            ParentId = page.ParentId,
            AppId = page.AppId,
            Order = page.Order,
            ShowOnMenus = page.ShowOnMenus,
            Name = page.Name,
            LastUpdated = page.LastUpdated,
            LastUpdatedBy = page.LastUpdatedBy,
            CreatedBy = page.CreatedBy,
            Path = page.Path,
            ResourceKey = page.ResourceKey,
            Layout = page.Layout
        };

        newPage.ParentId = parent?.Id;
        newPage.Parent = null;

        newPage.PageInfo = page.PageInfo.Select(selector: (PageInfo info) => new PageInfo
        {
            Id = 0,
            CultureId = info.CultureId,
            Description = info.Description,
            Keywords = info.Keywords,
            Title = info.Title
        })
            .ToList();

        newPage.Contents = (page.Contents ?? new List<Content>()).Select(selector: (Content content) => new Content
        {
            Id = 0,
            CultureId = content.CultureId,
            Name = content.Name,
            Html = content.Html
        })
            .ToList();

        newPage.Roles = ResolveRolesForNewPage(page: page, parent: parent)
            .Select(selector: role => new PageRole
            {
                RoleId = role.RoleId
            })
            .ToList();

        return await service.AddAsync(page: newPage);
    }

    public async ValueTask<IEnumerable<Result<Page>>> AddOrUpdate(IEnumerable<Page> items)
    {
        ValidatePages(pages: items, parameterName: "items");
        List<Result<Page>> results = new List<Result<Page>>();

        foreach (Page item in items)
        {
            try
            {
                Page savedItem = item.Id < 1 ? await AddAsync(page: item) : await UpdateAsync(page: item);

                results.Add(item: new Result<Page>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Page>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Page> items)
    {
        ValidatePages(pages: items, parameterName: "items");

        foreach (Page item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    public async ValueTask RecomputeAllForAppAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        if (!authorizationBroker.IsAdminOfApp(appId: appId))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await RecomputePathsAsync(appId: appId);
    }

    private async ValueTask RecomputePathsAsync(int appId)
    {
        Page[] pages = service.GetAll(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .OrderBy(keySelector: page => page.Order)
            .ToArray();

        await RecomputePathsAsync(parentId: null, parentPath: null, pages: pages);
    }

    private async ValueTask RecomputePathsAsync(
        int? parentId,
        string parentPath,
        IEnumerable<Page> pages)
    {
        foreach (Page page in pages
            .Where(predicate: item => item.ParentId == parentId)
            .OrderBy(keySelector: item => item.Order))
        {
            string newPath = BuildPath(pageName: page.Name, parentPath: parentPath);

            if (!string.Equals(a: page.Path, b: newPath, comparisonType: StringComparison.Ordinal))
            {
                page.Path = newPath;
                await service.UpdateAsync(page: page);
            }

            await RecomputePathsAsync(parentId: page.Id, parentPath: newPath, pages: pages);
        }
    }

    private ICollection<PageRole> ResolveRolesForNewPage(Page page, Page parent)
    {
        if ((page.Roles ?? Array.Empty<PageRole>()).Any())
        {
            return page.Roles;
        }

        return (parent != null)
            ? (parent.Roles ?? Array.Empty<PageRole>())
                .Select(selector: (PageRole role) => new PageRole
                {
                    RoleId = role.RoleId
                })
            .ToArray()
            : ((User?.Roles ?? Array.Empty<UserRole>())
                .Where(predicate: userRole => userRole.Role?.AppId == page.AppId)
            .Select(selector: userRole => new PageRole
            {
                RoleId = userRole.RoleId
            }))
                .ToArray();
    }

    private bool UserCan(string privKey, int pageId)
    {
        Page page = service.GetAll(ignoreFilters: false)
            .Where(predicate: existingPage => existingPage.Id == pageId)
            .FirstOrDefault();

        return page != null && ContentManagementModelLogic.UserCan(page: page, user: User, privilege: privKey);
    }

    private static string BuildPath(string pageName, string parentPath)
    {
        if (string.Equals(a: pageName, b: "Home", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        string text = (pageName ?? string.Empty).Replace(oldValue: " ", newValue: string.Empty);

        if (string.IsNullOrWhiteSpace(value: parentPath))
        {
            return text;
        }

        return parentPath.TrimEnd(trimChar: '/') + "/" + text;
    }

    private static string GetParentPath(string path)
    {
        if (string.IsNullOrWhiteSpace(value: path))
        {
            return null;
        }

        string trimmedPath = path.Trim(trimChar: '/');
        int separatorIndex = trimmedPath.LastIndexOf(value: '/');
        return separatorIndex < 0 ? null : trimmedPath[..separatorIndex];
    }

    private void ValidatePathDoesNotExistForApp(Page page)
    {
        string normalizedPath = (page.Path ?? string.Empty).ToUpperInvariant();

        bool pathExists = service.GetAll(ignoreFilters: true)
            .Any(predicate: existingPage =>
                existingPage.AppId == page.AppId &&
                existingPage.Id != page.Id &&
                (existingPage.Path ?? string.Empty).ToUpper() == normalizedPath);

        if (pathExists)
        {
            throw new ValidationException(message: $"A page already exists for app {page.AppId} with path '{page.Path ?? string.Empty}'.");
        }
    }
}