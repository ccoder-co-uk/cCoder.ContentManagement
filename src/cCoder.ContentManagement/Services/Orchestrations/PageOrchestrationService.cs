// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageOrchestrationService(
    IPageProcessingService processingService,
    IPageEventProcessingService eventService,
    ILayoutProcessingService layoutProcessingService) : IPageOrchestrationService
{
    public Page Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.Get(id: id);
    }

    public IQueryable<Page> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Page> AddAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");
        ValidateSinglePage(page: entity, parameterName: "entity");
        ValidatePageCollections(page: entity, parameterName: "entity");
        ValidateLayoutExistsForApp(page: entity);

        Page result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePageAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Page> UpdateAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");
        ValidatePageLayout(page: entity);
        ValidateLayoutExistsForApp(page: entity);

        Page result = await processingService.UpdateAsync(entity: entity);
        entity.Id = result.Id;
        entity.AppId = result.AppId;
        entity.ParentId = result.ParentId;
        entity.Path = result.Path;
        entity.Name = result.Name;
        entity.Layout = result.Layout;
        entity.ResourceKey = result.ResourceKey;
        entity.Order = result.Order;
        entity.ShowOnMenus = result.ShowOnMenus;
        entity.CreatedBy = result.CreatedBy;
        entity.CreatedOn = result.CreatedOn;
        entity.LastUpdated = result.LastUpdated;
        entity.LastUpdatedBy = result.LastUpdatedBy;
        await eventService.RaisePageUpdateEventAsync(entity: entity);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Page entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: page => page.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Page[] pagesToDelete =
            [.. GetAll(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray()
            .OrderByDescending(keySelector: page => GetPathDepth(path: page.Path))
            .ThenByDescending(keySelector: page => page.Order)];

        if (pagesToDelete.Length > 0)
        {
            await DeleteAllAsync(items: pagesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Page>>> AddOrUpdate(IEnumerable<Page> items)
    {
        Page[] pages = [.. ValidatePages(pages: items, parameterName: "items")
            .OrderBy(keySelector: page => GetPathDepth(path: page.Path))
            .ThenBy(keySelector: page => page.Order)];

        List<Result<Page>> results = new();

        foreach (Page page in pages)
        {
            try
            {
                Page result = page.Id <= 0
                    ? await AddAsync(entity: page)
                    : await UpdateAsync(entity: page);

                results.Add(item: new Result<Page>
                {
                    Success = true,
                    Item = result,
                    Message = page.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Page>
                {
                    Success = false,
                    Item = page,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportPagesAsync(int appId, Page[] pages)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Page[] validatedPages = ValidatePages(pages: pages, parameterName: "pages")
            .ToArray();

        Array.Sort(array: validatedPages,
comparison: (left, right) => left.Path.Split(separator: '/')
            .Length.CompareTo(value: right.Path.Split(separator: '/')
            .Length));

        Page[] allPages = processingService.GetAll(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray();

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

            await AddOrUpdate(items: [page]);
        }
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Page> items)
    {
        Page[] pages = ValidatePages(pages: items, parameterName: "items")
            .ToArray();

        foreach (Page page in pages)
        {
            await DeleteAsync(id: page.Id);
        }
    }

    public ValueTask RecomputeAllForAppAsync(int appId) =>
        processingService.RecomputeAllForAppAsync(appId: ValidateAppId(appId: appId, parameterName: "appId"));

    public Page GetRoot(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.GetRoot(id: id);
    }

    public IEnumerable<Page> GetChildren(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.GetChildren(id: id);
    }

    public string MenuFor(int id, string culture)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.MenuFor(id: id, culture: culture);
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

    private static int GetPathDepth(string path) =>
        string.IsNullOrWhiteSpace(value: path)
            ? 0
            : path.Trim(trimChar: '/')
        .Split(separator: '/', options: StringSplitOptions.RemoveEmptyEntries)
        .Length;

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

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
        bool layoutExists = layoutProcessingService.GetAll(ignoreFilters: true)
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
}