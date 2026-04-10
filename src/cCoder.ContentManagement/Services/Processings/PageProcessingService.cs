using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Content = cCoder.Data.Models.CMS.Content;
using Page = cCoder.Data.Models.CMS.Page;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using PageRole = cCoder.Data.Models.Security.PageRole;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService(
    IPageService service,
    IAuthorizationBroker authorizationBroker) : IPageProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public Page Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Page> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public string MenuFor(int id, string culture)
    {
        ValidateId(id, "id");
        IEnumerable<string> enumerable = from s in service.GetAll(ignoreFilters: false)
                                         where s.ParentId == id && s.ShowOnMenus
                                         orderby s.Order
                                         select $"<li data-id='{s.Id}' class='item'><a href='/{s.Path}'>{ContentManagementModelLogic.Title(s, culture)}</a></li>";
        string text = (enumerable.Any() ? string.Join("", enumerable) : string.Empty);
        return "<ul class='submenu'>" + text + "</ul>";
    }

    public Page GetRoot(int id)
    {
        ValidateId(id, "id");
        Page page = Get(id);
        while (page.ParentId.HasValue)
        {
            Page page2 = Get(page.ParentId.Value);
            page = page2 ?? page;
        }
        return page;
    }

    public IEnumerable<Page> GetChildren(int id)
    {
        ValidateId(id, "id");
        return from p in GetAll()
               where p.ParentId == (int?)id
               select p;
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        if (!UserCan("page_delete", id))
        {
            throw new SecurityException("Access Denied!");
        }
        return service.DeleteAsync(id);
    }

    public async ValueTask<Page> UpdateAsync(Page page)
    {
        ValidatePage(page, "page");
        Page dbVersion = service.GetAll(ignoreFilters: true)
            .Where(existingPage => existingPage.Id == page.Id)
            .FirstOrDefault();
        if (dbVersion == null || !UserCan("page_update", dbVersion.Id))
        {
            throw new SecurityException("Access Denied!");
        }

        Page parent = page.ParentId.HasValue
            ? service.GetAll(ignoreFilters: true)
                .Where(existingPage => existingPage.Id == page.ParentId.Value)
                .FirstOrDefault()
            : null;

        if (page.ParentId.HasValue && parent == null)
        {
            throw new SecurityException("Access Denied!");
        }

        dbVersion.ParentId = page.ParentId;
        dbVersion.AppId = page.AppId;
        dbVersion.Order = page.Order;
        dbVersion.ShowOnMenus = page.ShowOnMenus;
        dbVersion.Name = page.Name;
        dbVersion.ResourceKey = page.ResourceKey;
        dbVersion.Layout = page.Layout;
        dbVersion.Path = BuildPath(page.Name, parent?.Path);

        return await service.UpdateAsync(dbVersion);
    }

    public async ValueTask<Page> AddAsync(Page page)
    {
        ValidatePage(page, "page");
        if (!authorizationBroker.IsAdminOfApp(page.AppId) && page.ParentId.HasValue)
        {
            UserCan("page_create", page.ParentId.Value);
        }
        Page parent = null;
        if (page.ParentId.HasValue)
        {
            parent = service.GetAll(ignoreFilters: false)
                .Where(existingPage => existingPage.Id == page.ParentId.Value)
                .FirstOrDefault();
        }
        else if (page.Path != null && page.Path.Contains('/'))
        {
            string parentPath = GetParentPath(page.Path);
            string normalizedParentPath = parentPath.TrimStart('/').ToLower();
            parent = service.GetAll(ignoreFilters: false)
                .Where(existingPage =>
                    existingPage.AppId == page.AppId &&
                    existingPage.Path.ToLower() == normalizedParentPath)
                .FirstOrDefault();
        }
        page.Path = BuildPath(page.Name, parent?.Path);
        page.ParentId = parent?.Id;
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
        newPage.PageInfo = page.PageInfo.Select((PageInfo info) => new PageInfo
        {
            Id = 0,
            CultureId = info.CultureId,
            Description = info.Description,
            Keywords = info.Keywords,
            Title = info.Title
        }).ToList();
        newPage.Contents = (page.Contents ?? new List<Content>()).Select((Content content) => new Content
        {
            Id = 0,
            CultureId = content.CultureId,
            Name = content.Name,
            Html = content.Html
        }).ToList();
        newPage.Roles = (from role in ResolveRolesForNewPage(page, parent)
                         select new PageRole
                         {
                             RoleId = role.RoleId
                         }).ToList();
        return await service.AddAsync(newPage);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Page>>> AddOrUpdate(IEnumerable<Page> items)
    {
        ValidatePages(items, "items");
        List<cCoder.ContentManagement.Models.Result<Page>> results = new List<cCoder.ContentManagement.Models.Result<Page>>();
        foreach (Page item in items)
        {
            try
            {
                Page savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Page>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Page>
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
        ValidatePages(items, "items");
        foreach (Page item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    public async ValueTask RecomputeAllForAppAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        if (!authorizationBroker.IsAdminOfApp(appId))
        {
            throw new SecurityException("Access Denied!");
        }

        await RecomputePathsAsync(appId);
    }

    private async ValueTask RecomputePathsAsync(int appId)
    {
        Page[] pages = service.GetAll(ignoreFilters: true)
            .Where(page => page.AppId == appId)
            .OrderBy(page => page.Order)
            .ToArray();

        await RecomputePathsAsync(parentId: null, parentPath: null, pages);
    }

    private async ValueTask RecomputePathsAsync(
        int? parentId,
        string parentPath,
        IEnumerable<Page> pages)
    {
        foreach (Page page in pages
            .Where(item => item.ParentId == parentId)
            .OrderBy(item => item.Order))
        {
            string newPath = BuildPath(page.Name, parentPath);

            if (!string.Equals(page.Path, newPath, StringComparison.Ordinal))
            {
                page.Path = newPath;
                await service.UpdateAsync(page);
            }

            await RecomputePathsAsync(page.Id, newPath, pages);
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
                .Select((PageRole role) => new PageRole
                {
                    RoleId = role.RoleId
                })
                .ToArray()
            : ((User?.Roles ?? Array.Empty<cCoder.Data.Models.Security.UserRole>())
                .Where(userRole => userRole.Role?.AppId == page.AppId)
                .Select(userRole => new PageRole
                {
                    RoleId = userRole.RoleId
                }))
                .ToArray();
    }

    private bool UserCan(string privKey, int pageId)
    {
        Page page = service.GetAll(ignoreFilters: false)
            .Where(existingPage => existingPage.Id == pageId)
            .FirstOrDefault();

        return page != null && ContentManagementModelLogic.UserCan(page, User, privKey);
    }

    private static string BuildPath(string pageName, string parentPath)
    {
        if (string.Equals(pageName, "Home", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }
        string text = (pageName ?? string.Empty).Replace(" ", string.Empty);
        if (string.IsNullOrWhiteSpace(parentPath))
        {
            return text;
        }
        return parentPath.TrimEnd('/') + "/" + text;
    }

    private static string GetParentPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        string trimmedPath = path.Trim('/');
        int separatorIndex = trimmedPath.LastIndexOf('/');
        return separatorIndex < 0 ? null : trimmedPath[..separatorIndex];
    }

}

