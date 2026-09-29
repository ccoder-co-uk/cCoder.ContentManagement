// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageProcessingService(
    IPageService service) : IPageProcessingService
{
    public bool LayoutExistsForApp(int appId, string layoutName) =>
        TryCatch<bool>(operation: () =>
        {
            ValidateLayoutExistsForAppOnGet(inputs: [appId, layoutName]);
            ValidateAppId(appId: appId, parameterName: "appId");
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: layoutName);
            return service.LayoutExistsForApp(appId: appId, layoutName: layoutName);
        });

    public Page GetPage(int pageId) =>
        TryCatch<Page>(operation: () =>
    {
        ValidatePageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        return service.GetPage(pageId: pageId);

    });

    public IQueryable<Page> GetAllPage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Page>>(operation: () =>
    {
        ValidateAllPageOnGet(inputs: [ignoreFilters]);
        return service.GetAllPage(ignoreFilters: ignoreFilters);
    });

    public string MenuFor(int pageId, string culture) =>
        TryCatch<string>(operation: () =>
    {
        ValidateMenuFor(inputs: [pageId, culture]);
        ValidateId(pageId: pageId, parameterName: "id");

        IEnumerable<string> enumerable = service.GetAllPage(ignoreFilters: false)
            .Where(predicate: page => page.ParentId == pageId && page.ShowOnMenus)
            .OrderBy(keySelector: page => page.Order)
            .Select(selector: page =>
                $"<li data-id='{page.Id}' class='item'><a href='/{page.Path}'>{GetPageInfo(page: page, culture: culture).Title}</a></li>");

        string text = string.Join(separator: "", values: enumerable);
        return "<ul class='submenu'>" + text + "</ul>";

    });

    public Page GetRootPage(int pageId) =>
        TryCatch<Page>(operation: () =>
    {
        ValidateRootPageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        Page page = ExecuteGetPage(pageId: pageId);

        while (page.ParentId.HasValue)
        {
            Page page2 = ExecuteGetPage(pageId: page.ParentId.Value);
            page = page2 ?? page;
        }

        return page;

    });

    public IEnumerable<Page> GetChildrenPage(int pageId) =>
        TryCatch<IEnumerable<Page>>(operation: () =>
    {
        ValidateChildrenPageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");

        return ExecuteGetAllPage()
            .Where(predicate: page => page.ParentId == (int?)pageId);

    });

    public ValueTask DeleteAsync(int pageId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");

        return service.DeleteAsync(pageId: pageId);

    }, isValueTask: true);

    public ValueTask<Page> UpdatePageAsync(Page updatedPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnUpdate(inputs: [updatedPage]);
        ValidatePage(page: updatedPage, parameterName: "page");

        IQueryable<Page> pages = service.GetAllPage(ignoreFilters: true);

        Page dbVersion = pages
            .Where(predicate: existingPage => existingPage.Id == updatedPage.Id)
            .FirstOrDefault();

        if (dbVersion == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Page parent = updatedPage.ParentId.HasValue
            ? pages.Where(predicate: existingPage =>
                existingPage.Id == updatedPage.ParentId.Value)
            .FirstOrDefault()
            : null;

        if (updatedPage.ParentId.HasValue && parent == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await NormalizeSiblingOrdersAsync(
            storedPage: dbVersion,
            updatedPage: updatedPage,
            pages: pages);

        dbVersion.ParentId = updatedPage.ParentId;
        dbVersion.AppId = updatedPage.AppId;
        dbVersion.Order = updatedPage.Order;
        dbVersion.ShowOnMenus = updatedPage.ShowOnMenus;
        dbVersion.Name = updatedPage.Name;
        dbVersion.ResourceKey = updatedPage.ResourceKey;
        dbVersion.Layout = updatedPage.Layout;
        dbVersion.Path = BuildPath(pageName: updatedPage.Name, parentPath: parent?.Path);

        return await service.UpdatePageAsync(updatedPage: dbVersion);

    }, isValueTask: true);

    public ValueTask<Page> AddPageAsync(Page newPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnAdd(inputs: [newPage]);
        ValidatePage(page: newPage, parameterName: "page");

        Page parent = null;

        if (newPage.ParentId.HasValue)
        {
            parent = service.GetAllPage(ignoreFilters: false)
                .Where(predicate: existingPage => existingPage.Id == newPage.ParentId.Value)
                .FirstOrDefault();
        }
        else
        {
            if (newPage.Path != null && newPage.Path.Contains(value: '/'))
            {
                string parentPath = GetParentPath(path: newPage.Path);

                string normalizedParentPath = parentPath.TrimStart(trimChar: '/')
                    .ToLower();

                parent = service.GetAllPage(ignoreFilters: false)
                    .Where(predicate: existingPage =>
                        existingPage.AppId == newPage.AppId &&
                        existingPage.Path.ToLower() == normalizedParentPath)
                    .FirstOrDefault();
            }
        }

        newPage.Path = BuildPath(pageName: newPage.Name, parentPath: parent?.Path);
        newPage.ParentId = parent?.Id;
        ValidatePathDoesNotExistForApp(page: newPage);

        Page storagePage = new Page
        {
            ParentId = newPage.ParentId,
            AppId = newPage.AppId,
            Order = newPage.Order,
            ShowOnMenus = newPage.ShowOnMenus,
            Name = newPage.Name,
            LastUpdated = newPage.LastUpdated,
            LastUpdatedBy = newPage.LastUpdatedBy,
            CreatedBy = newPage.CreatedBy,
            CreatedOn = newPage.CreatedOn,
            Path = newPage.Path,
            ResourceKey = newPage.ResourceKey,
            Layout = newPage.Layout
        };

        storagePage.ParentId = parent?.Id;
        storagePage.Parent = null;

        storagePage.PageInfo = ClonePageInfo(
            pageInfo: newPage.PageInfo);

        storagePage.Contents = CloneContent(
            contents: newPage.Contents);

        storagePage.Roles = ClonePageRoles(
            pageRoles: ResolveRolesForNewPage(
                page: newPage,
                parent: parent));

        return await service.AddPageAsync(newPage: storagePage);

    }, isValueTask: true);

    public ValueTask<Page> ImportPageAsync(Page page) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePage(page: page, parameterName: "page");

        page.Path = (page.Path ?? string.Empty)
            .TrimStart(trimChar: '/');

        ValidatePathDoesNotExistForApp(page: page);

        return page.Id <= 0
            ? await service.AddPageAsync(newPage: page)
            : await service.UpdatePageAsync(updatedPage: page);
    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Page>>> AddOrUpdatePageResult(IEnumerable<Page> newPage) =>
        TryCatch<IEnumerable<OperationResult<Page>>>(operation: async () =>
    {
        ValidateOrUpdatePageResultOnAdd(inputs: [newPage]);
        ValidatePages(pages: newPage, parameterName: "items");
        List<OperationResult<Page>> results = new List<OperationResult<Page>>();

        foreach (Page item in newPage)
        {
            try
            {
                Page savedItem = item.Id < 1 ? await ExecuteAddPageAsync(page: item) : await ExecuteUpdatePageAsync(updatedPage: item);

                results.Add(item: new OperationResult<Page>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Page>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllPageAsync(IEnumerable<Page> deletedPage) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageOnDelete(inputs: [deletedPage]);
        ValidatePages(pages: deletedPage, parameterName: "items");

        foreach (Page item in deletedPage)
        {
            await ExecuteDeleteAsync(pageId: item.Id);
        }

    }, isValueTask: true);

    public ValueTask RecomputeAllForAppAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateRecomputeAllForAppAsync(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        await RecomputePathsAsync(appId: appId);

    }, isValueTask: true);

    private async ValueTask RecomputePathsAsync(int appId)
    {
        IQueryable<Page> pagesQuery = service.GetAllPage(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .OrderBy(keySelector: page => page.Order);

        List<Page> pages = [];

        foreach (Page page in pagesQuery)
        {
            pages.Add(item: page);
        }

        await RecomputePathsAsync(parentId: null, parentPath: null, pages: pages);
    }

    private async ValueTask RecomputePathsAsync(
        int? parentId,
        string parentPath,
        IEnumerable<Page> pages)
    {
        foreach (Page page in pages)
        {
            if (page.ParentId != parentId)
            {
                continue;
            }

            string newPath = BuildPath(pageName: page.Name, parentPath: parentPath);

            if (!string.Equals(a: page.Path, b: newPath, comparisonType: StringComparison.Ordinal))
            {
                page.Path = newPath;
                await service.UpdatePageAsync(updatedPage: page);
            }

            await RecomputePathsAsync(parentId: page.Id, parentPath: newPath, pages: pages);
        }
    }

    private ICollection<PageRole> ResolveRolesForNewPage(Page page, Page parent)
    {
        if (page.Roles?.Count > 0)
        {
            return page.Roles;
        }

        return parent is null
            ? []
            : ClonePageRoles(pageRoles: parent.Roles);
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

        bool pathExists = service.GetAllPage(ignoreFilters: true)
            .Any(predicate: existingPage =>
                existingPage.AppId == page.AppId &&
                existingPage.Id != page.Id &&
                (existingPage.Path ?? string.Empty).ToUpper() == normalizedPath);

        if (pathExists)
        {
            throw new ValidationException(message: $"A page already exists for app {page.AppId} with path '{page.Path ?? string.Empty}'.");
        }
    }

    private async ValueTask<Page> ExecuteAddPageAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");

        Page parent = null;

        if (page.ParentId.HasValue)
        {
            parent = service.GetAllPage(ignoreFilters: false)
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

                parent = service.GetAllPage(ignoreFilters: false)
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
            CreatedOn = page.CreatedOn,
            Path = page.Path,
            ResourceKey = page.ResourceKey,
            Layout = page.Layout
        };

        newPage.ParentId = parent?.Id;
        newPage.Parent = null;

        newPage.PageInfo = ClonePageInfo(pageInfo: page.PageInfo);

        newPage.Contents = CloneContent(contents: page.Contents);

        newPage.Roles = ClonePageRoles(
            pageRoles: ResolveRolesForNewPage(
                page: page,
                parent: parent));

        return await service.AddPageAsync(newPage: newPage);
    }

    private ValueTask ExecuteDeleteAsync(int pageId)
    {
        ValidateId(pageId: pageId, parameterName: "id");

        return service.DeleteAsync(pageId: pageId);
    }

    private IQueryable<Page> ExecuteGetAllPage(bool ignoreFilters = false) =>
        service.GetAllPage(ignoreFilters: ignoreFilters);

    private Page ExecuteGetPage(int pageId)
    {
        ValidateId(pageId: pageId, parameterName: "id");
        return service.GetPage(pageId: pageId);
    }

    private async ValueTask<Page> ExecuteUpdatePageAsync(Page updatedPage)
    {
        ValidatePage(page: updatedPage, parameterName: "page");

        IQueryable<Page> pages = service.GetAllPage(ignoreFilters: true);

        Page dbVersion = pages
            .Where(predicate: existingPage => existingPage.Id == updatedPage.Id)
            .FirstOrDefault();

        if (dbVersion == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Page parent = updatedPage.ParentId.HasValue
            ? pages.Where(predicate: existingPage =>
                existingPage.Id == updatedPage.ParentId.Value)
            .FirstOrDefault()
            : null;

        if (updatedPage.ParentId.HasValue && parent == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await NormalizeSiblingOrdersAsync(
            storedPage: dbVersion,
            updatedPage: updatedPage,
            pages: pages);

        dbVersion.ParentId = updatedPage.ParentId;
        dbVersion.AppId = updatedPage.AppId;
        dbVersion.Order = updatedPage.Order;
        dbVersion.ShowOnMenus = updatedPage.ShowOnMenus;
        dbVersion.Name = updatedPage.Name;
        dbVersion.ResourceKey = updatedPage.ResourceKey;
        dbVersion.Layout = updatedPage.Layout;
        dbVersion.Path = BuildPath(pageName: updatedPage.Name, parentPath: parent?.Path);

        return await service.UpdatePageAsync(updatedPage: dbVersion);
    }

    private async ValueTask NormalizeSiblingOrdersAsync(
        Page storedPage,
        Page updatedPage,
        IQueryable<Page> pages)
    {
        bool parentChanged = storedPage.ParentId != updatedPage.ParentId;

        if (!parentChanged && storedPage.Order == updatedPage.Order)
        {
            return;
        }

        foreach (Page sibling in pages)
        {
            if (sibling.Id == storedPage.Id
                || sibling.AppId != storedPage.AppId)
            {
                continue;
            }

            bool orderChanged = parentChanged
                ? AdjustOrderAcrossParents(
                    sibling: sibling,
                    storedPage: storedPage,
                    updatedPage: updatedPage)
                : AdjustOrderWithinParent(
                    sibling: sibling,
                    storedPage: storedPage,
                    updatedPage: updatedPage);

            if (orderChanged)
            {
                await service.UpdatePageAsync(updatedPage: sibling);
            }
        }
    }

    private static bool AdjustOrderAcrossParents(
        Page sibling,
        Page storedPage,
        Page updatedPage)
    {
        if (sibling.ParentId == storedPage.ParentId
            && sibling.Order > storedPage.Order)
        {
            sibling.Order--;
            return true;
        }

        if (sibling.ParentId == updatedPage.ParentId
            && sibling.Order >= updatedPage.Order)
        {
            sibling.Order++;
            return true;
        }

        return false;
    }

    private static bool AdjustOrderWithinParent(
        Page sibling,
        Page storedPage,
        Page updatedPage)
    {
        if (sibling.ParentId != storedPage.ParentId)
        {
            return false;
        }

        if (updatedPage.Order < storedPage.Order
            && sibling.Order >= updatedPage.Order
            && sibling.Order < storedPage.Order)
        {
            sibling.Order++;
            return true;
        }

        if (updatedPage.Order > storedPage.Order
            && sibling.Order > storedPage.Order
            && sibling.Order <= updatedPage.Order)
        {
            sibling.Order--;
            return true;
        }

        return false;
    }

    private static PageInfo GetPageInfo(Page page, string culture)
    {
        culture ??= string.Empty;

        if (page?.PageInfo == null || page.PageInfo.Count == 0)
        {
            return new PageInfo
            {
                CultureId = culture,
                Title = page?.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
        }

        PageInfo fallback = null;
        PageInfo match = null;

        foreach (PageInfo info in page.PageInfo)
        {
            if (fallback is null
                || (info.CultureId?.Length ?? 0)
                    > (fallback.CultureId?.Length ?? 0))
            {
                fallback = info;
            }

            if (culture == info.CultureId
                || culture.Contains(value: info.CultureId ?? string.Empty))
            {
                if (match is null
                    || (info.CultureId?.Length ?? 0)
                        > (match.CultureId?.Length ?? 0))
                {
                    match = info;
                }
            }
        }

        return match
            ?? fallback
            ?? new PageInfo
            {
                CultureId = culture,
                Title = page.Name ?? string.Empty,
                Description = string.Empty,
                Keywords = string.Empty
            };
    }

    private static List<PageInfo> ClonePageInfo(
        IEnumerable<PageInfo> pageInfo)
    {
        List<PageInfo> results = [];

        foreach (PageInfo info in pageInfo ?? [])
        {
            results.Add(item: new PageInfo
            {
                Id = 0,
                CultureId = info.CultureId ?? string.Empty,
                Description = info.Description,
                Keywords = info.Keywords,
                Title = info.Title
            });
        }

        return results;
    }

    private static List<Content> CloneContent(
        IEnumerable<Content> contents)
    {
        List<Content> results = [];

        foreach (Content content in contents ?? [])
        {
            results.Add(item: new Content
            {
                Id = 0,
                CultureId = content.CultureId ?? string.Empty,
                Name = content.Name,
                Html = content.Html
            });
        }

        return results;
    }

    private static List<PageRole> ClonePageRoles(
        IEnumerable<PageRole> pageRoles)
    {
        List<PageRole> results = [];

        foreach (PageRole role in pageRoles ?? [])
        {
            results.Add(item: new PageRole
            {
                RoleId = role.RoleId
            });
        }

        return results;
    }
}