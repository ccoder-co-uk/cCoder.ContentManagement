// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageOrchestrationService(
    IPageProcessingService processingService,
    IPageEventProcessingService eventService,
    ILayoutProcessingService layoutProcessingService) : IPageOrchestrationService
{
    public ValueTask<Page> GetPageForRenderAsync(int pageId) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageForRenderOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");

        return await processingService.GetPageForRenderAsync(
            pageId: pageId);
    }, isValueTask: true);

    public Page GetPage(int pageId) =>
        TryCatch<Page>(operation: () =>
    {
        ValidatePageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        return processingService.GetPage(pageId: pageId);

    });

    public IQueryable<Page> GetAllPage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Page>>(operation: () =>
    {
        ValidateAllPageOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPage(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Page> AddPageAsync(Page newPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnAdd(inputs: [newPage]);
        ValidatePage(page: newPage, parameterName: "entity");
        ValidateSinglePage(page: newPage, parameterName: "entity");
        ValidatePageCollections(page: newPage, parameterName: "entity");
        ValidateLayoutExistsForApp(page: newPage);

        Page result = await processingService.AddPageAsync(newPage: newPage);
        await eventService.RaisePageAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Page> UpdatePageAsync(Page updatedPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnUpdate(inputs: [updatedPage]);
        ValidatePage(page: updatedPage, parameterName: "entity");
        ValidatePageLayout(page: updatedPage);
        ValidateLayoutExistsForApp(page: updatedPage);

        Page result = await processingService.UpdatePageAsync(updatedPage: updatedPage);
        updatedPage.Id = result.Id;
        updatedPage.AppId = result.AppId;
        updatedPage.ParentId = result.ParentId;
        updatedPage.Path = result.Path;
        updatedPage.Name = result.Name;
        updatedPage.Layout = result.Layout;
        updatedPage.ResourceKey = result.ResourceKey;
        updatedPage.Order = result.Order;
        updatedPage.ShowOnMenus = result.ShowOnMenus;
        updatedPage.CreatedBy = result.CreatedBy;
        updatedPage.CreatedOn = result.CreatedOn;
        updatedPage.LastUpdated = result.LastUpdated;
        updatedPage.LastUpdatedBy = result.LastUpdatedBy;
        await eventService.RaisePageUpdateEventAsync(entity: updatedPage);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int pageId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");

        Page entity;

        try
        {
            entity = processingService.GetPage(pageId: pageId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllPage(ignoreFilters: true)
                .FirstOrDefault(predicate: page => page.Id == pageId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(pageId: pageId);

    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateByAppIdOnDelete(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        Page[] pagesToDelete =
            [.. ExecuteGetAllPage(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray()
            .OrderByDescending(keySelector: page => GetPathDepth(path: page.Path))
            .ThenByDescending(keySelector: page => page.Order)];

        if (pagesToDelete.Length > 0)
        {
            await ExecuteDeleteAllPageAsync(deletedPage: pagesToDelete);
        }

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Page>>> AddOrUpdatePageResult(IEnumerable<Page> newPage) =>
        TryCatch<IEnumerable<OperationResult<Page>>>(operation: async () =>
    {
        ValidateOrUpdatePageResultOnAdd(inputs: [newPage]);

        Page[] pages = [.. ValidatePages(pages: newPage, parameterName: "items")
            .OrderBy(keySelector: page => GetPathDepth(path: page.Path))
            .ThenBy(keySelector: page => page.Order)];

        List<OperationResult<Page>> results = new();

        foreach (Page page in pages)
        {
            try
            {
                Page result = page.Id <= 0
                    ? await ExecuteAddPageAsync(newPage: page)
                    : await ExecuteUpdatePageAsync(updatedPage: page);

                results.Add(item: new OperationResult<Page>
                {
                    Success = true,
                    Item = result,
                    Message = page.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Page>
                {
                    Success = false,
                    Item = page,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask<Page[]> ImportPagesAsync(int appId, Page[] pages) =>
        TryCatch<Page[]>(operation: async () =>
    {
        ValidateImportPagesAsync(inputs: [appId, pages]);
        ValidateAppId(appId: appId, parameterName: "appId");

        Page[] validatedPages = ValidatePages(pages: pages, parameterName: "pages")
            .ToArray();

        Array.Sort(array: validatedPages,
comparison: (left, right) => left.Path.Split(separator: '/')
            .Length.CompareTo(value: right.Path.Split(separator: '/')
            .Length));

        Page[] allPages = processingService.GetAllPage(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray();

        List<Page> importedPages = new();

        foreach (Page page in validatedPages)
        {
            page.AppId = appId;

            string parentPath = GetParentPath(path: page.Path);

            Page parent = parentPath != null
                ? allPages.FirstOrDefault(predicate: existing =>
                    existing.Path.Equals(value: parentPath, comparisonType: StringComparison.OrdinalIgnoreCase)
                    && existing.AppId == appId)
                : null;

            page.ParentId = parent?.Id;

            page.Id = allPages.FirstOrDefault(predicate: existing =>
                existing.Path.Equals(value: page.Path.TrimStart(trimChar: '/'), comparisonType: StringComparison.OrdinalIgnoreCase)
                && existing.AppId == appId)?.Id ?? 0;

            IEnumerable<OperationResult<Page>> results =
                await processingService.AddOrUpdatePageResult(newPage: [page]);

            OperationResult<Page> result = results.Single();

            if (!result.Success)
            {
                throw new InvalidOperationException(message: result.Message);
            }

            importedPages.Add(item: result.Item);
        }

        return importedPages.ToArray();

    }, isValueTask: true);

    public ValueTask DeleteAllPageAsync(IEnumerable<Page> deletedPage) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageOnDelete(inputs: [deletedPage]);

        Page[] pages = ValidatePages(pages: deletedPage, parameterName: "items")
            .ToArray();

        foreach (Page page in pages)
        {
            await ExecuteDeleteAsync(pageId: page.Id);
        }

    }, isValueTask: true);

    public ValueTask RecomputeAllForAppAsync(int appId) =>
        TryCatch(operation: () =>
    {
        ValidateRecomputeAllForAppAsync(inputs: [appId]);
        return processingService.RecomputeAllForAppAsync(appId: ValidateAppId(appId: appId, parameterName: "appId"));
    }, isValueTask: true);

    public Page GetRootPage(int pageId) =>
        TryCatch<Page>(operation: () =>
    {
        ValidateRootPageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        return processingService.GetRootPage(pageId: pageId);

    });

    public IEnumerable<Page> GetChildrenPage(int pageId) =>
        TryCatch<IEnumerable<Page>>(operation: () =>
    {
        ValidateChildrenPageOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        return processingService.GetChildrenPage(pageId: pageId);

    });

    public string MenuFor(int pageId, string culture) =>
        TryCatch<string>(operation: () =>
    {
        ValidateMenuFor(inputs: [pageId, culture]);
        ValidateId(pageId: pageId, parameterName: "id");
        return processingService.MenuFor(pageId: pageId, culture: culture);

    });

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

    private static int GetPathDepth(string path) =>
        string.IsNullOrWhiteSpace(value: path)
            ? 0
            : path.Trim(trimChar: '/')
        .Split(separator: '/', options: StringSplitOptions.RemoveEmptyEntries)
        .Length;

    private static void ValidateId(int pageId, string parameterName) =>
        ThrowIf(condition: pageId < 1, message: parameterName + " must be greater than 0.");

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return page;
    }

    private static IEnumerable<Page> ValidatePages(IEnumerable<Page> pages, string parameterName)
    {
        if (pages == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pages;
    }

    private static void ValidateSinglePage(Page page, string parameterName) =>
        ThrowIf(condition: page.Pages != null && page.Pages.Any(), message: "Can only import one page at a time.");

    private static void ValidatePageCollections(Page page, string parameterName)
    {
        if (page.PageInfo == null || !page.PageInfo.Any(predicate: pi => pi.CultureId == string.Empty))
        {
            throw new ValidationException(message: "Pages MUST have page information defined for the default culture, other cultures are optional.");
        }

        ValidatePageLayout(page: page);

        if (page.Contents == null)
        {
            throw new ValidationException(message: "Pages MUST include a contents collection.");
        }
    }

    private void ValidateLayoutExistsForApp(Page page)
    {
        bool layoutExists = layoutProcessingService.GetAllLayout(ignoreFilters: true)
            .Any(predicate: layout =>
                layout.AppId == page.AppId &&
                layout.Name == page.Layout);

        if (!layoutExists)
        {
            throw new ValidationException(message: $"Layout '{page.Layout}' does not exist for app {page.AppId}.");
        }
    }

    private static void ValidatePageLayout(Page page) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: page.Layout), message: "Pages MUST specify a layout.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private async ValueTask<IEnumerable<OperationResult<Page>>> ExecuteAddOrUpdatePageResult(IEnumerable<Page> newPage)
    {
        Page[] pages = [.. ValidatePages(pages: newPage, parameterName: "items")
            .OrderBy(keySelector: page => GetPathDepth(path: page.Path))
            .ThenBy(keySelector: page => page.Order)];

        List<OperationResult<Page>> results = new();

        foreach (Page page in pages)
        {
            try
            {
                Page result = page.Id <= 0
                    ? await ExecuteAddPageAsync(newPage: page)
                    : await ExecuteUpdatePageAsync(updatedPage: page);

                results.Add(item: new OperationResult<Page>
                {
                    Success = true,
                    Item = result,
                    Message = page.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Page>
                {
                    Success = false,
                    Item = page,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private async ValueTask<Page> ExecuteAddPageAsync(Page newPage)
    {
        ValidatePage(page: newPage, parameterName: "entity");
        ValidateSinglePage(page: newPage, parameterName: "entity");
        ValidatePageCollections(page: newPage, parameterName: "entity");
        ValidateLayoutExistsForApp(page: newPage);

        Page result = await processingService.AddPageAsync(newPage: newPage);
        await eventService.RaisePageAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask ExecuteDeleteAllPageAsync(IEnumerable<Page> deletedPage)
    {
        Page[] pages = ValidatePages(pages: deletedPage, parameterName: "items")
            .ToArray();

        foreach (Page page in pages)
        {
            await ExecuteDeleteAsync(pageId: page.Id);
        }
    }

    private async ValueTask ExecuteDeleteAsync(int pageId)
    {
        ValidateId(pageId: pageId, parameterName: "id");

        Page entity;

        try
        {
            entity = processingService.GetPage(pageId: pageId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllPage(ignoreFilters: true)
                .FirstOrDefault(predicate: page => page.Id == pageId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(pageId: pageId);
    }

    private IQueryable<Page> ExecuteGetAllPage(bool ignoreFilters = false) =>
        processingService.GetAllPage(ignoreFilters: ignoreFilters);

    private async ValueTask<Page> ExecuteUpdatePageAsync(Page updatedPage)
    {
        ValidatePage(page: updatedPage, parameterName: "entity");
        ValidatePageLayout(page: updatedPage);
        ValidateLayoutExistsForApp(page: updatedPage);

        Page result = await processingService.UpdatePageAsync(updatedPage: updatedPage);
        updatedPage.Id = result.Id;
        updatedPage.AppId = result.AppId;
        updatedPage.ParentId = result.ParentId;
        updatedPage.Path = result.Path;
        updatedPage.Name = result.Name;
        updatedPage.Layout = result.Layout;
        updatedPage.ResourceKey = result.ResourceKey;
        updatedPage.Order = result.Order;
        updatedPage.ShowOnMenus = result.ShowOnMenus;
        updatedPage.CreatedBy = result.CreatedBy;
        updatedPage.CreatedOn = result.CreatedOn;
        updatedPage.LastUpdated = result.LastUpdated;
        updatedPage.LastUpdatedBy = result.LastUpdatedBy;
        await eventService.RaisePageUpdateEventAsync(entity: updatedPage);
        return result;
    }
}