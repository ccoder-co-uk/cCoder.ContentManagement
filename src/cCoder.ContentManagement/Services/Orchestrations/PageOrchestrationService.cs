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
        ValidateId(id, "id");
        return processingService.Get(id);
    }

    public IQueryable<Page> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Page> AddAsync(Page entity)
    {
        ValidatePage(entity, "entity");
        ValidateSinglePage(entity, "entity");
        ValidatePageCollections(entity, "entity");
        ValidateLayoutExistsForApp(entity);

        Page result = await processingService.AddAsync(entity);
        await eventService.RaisePageAddEventAsync(result);
        return result;
    }

    public async ValueTask<Page> UpdateAsync(Page entity)
    {
        ValidatePage(entity, "entity");
        ValidatePageLayout(entity);
        ValidateLayoutExistsForApp(entity);

        Page result = await processingService.UpdateAsync(entity);
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
        await eventService.RaisePageUpdateEventAsync(entity);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Page entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(page => page.Id == id);
        }

        if (entity == null)
            return;

        await eventService.RaisePageDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Page[] pagesToDelete =
            [.. GetAll(ignoreFilters: true)
                .Where(page => page.AppId == appId)
                .ToArray()
                .OrderByDescending(page => GetPathDepth(page.Path))
                .ThenByDescending(page => page.Order)];

        if (pagesToDelete.Length > 0)
            await DeleteAllAsync(pagesToDelete);
    }

    public async ValueTask<IEnumerable<Result<Page>>> AddOrUpdate(IEnumerable<Page> items)
    {
        Page[] pages = [.. ValidatePages(items, "items")
            .OrderBy(page => GetPathDepth(page.Path))
            .ThenBy(page => page.Order)];
        List<Result<Page>> results = new();

        foreach (Page page in pages)
        {
            try
            {
                Page result = page.Id <= 0
                    ? await AddAsync(page)
                    : await UpdateAsync(page);

                results.Add(new Result<Page>
                {
                    Success = true,
                    Item = result,
                    Message = page.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Page>
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
        ValidateAppId(appId, "appId");

        Page[] validatedPages = ValidatePages(pages, "pages").ToArray();
        Array.Sort(validatedPages,
            (left, right) => left.Path.Split('/').Length.CompareTo(right.Path.Split('/').Length));

        Page[] allPages = processingService.GetAll(ignoreFilters: true)
            .Where(page => page.AppId == appId)
            .ToArray();

        foreach (Page page in validatedPages)
        {
            page.AppId = appId;

            string parentPath = GetParentPath(page.Path);
            Page parent = parentPath != null
                ? allPages.FirstOrDefault(existing =>
                    existing.Path.Equals(parentPath, StringComparison.OrdinalIgnoreCase)
                    && existing.AppId == appId)
                : null;

            page.ParentId = parent?.Id;
            page.Id = allPages.FirstOrDefault(existing =>
                existing.Path.Equals(page.Path.TrimStart('/'), StringComparison.OrdinalIgnoreCase)
                && existing.AppId == appId)?.Id ?? 0;

            await AddOrUpdate([page]);
        }
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Page> items)
    {
        Page[] pages = ValidatePages(items, "items").ToArray();

        foreach (Page page in pages)
            await DeleteAsync(page.Id);
    }

    public ValueTask RecomputeAllForAppAsync(int appId) =>
        processingService.RecomputeAllForAppAsync(ValidateAppId(appId, "appId"));

    public Page GetRoot(int id)
    {
        ValidateId(id, "id");
        return processingService.GetRoot(id);
    }

    public IEnumerable<Page> GetChildren(int id)
    {
        ValidateId(id, "id");
        return processingService.GetChildren(id);
    }

    public string MenuFor(int id, string culture)
    {
        ValidateId(id, "id");
        return processingService.MenuFor(id, culture);
    }

    private static string GetParentPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        string trimmedPath = path.Trim('/');
        int separatorIndex = trimmedPath.LastIndexOf('/');
        return separatorIndex < 0 ? null : trimmedPath[..separatorIndex];
    }

    private static int GetPathDepth(string path) =>
        string.IsNullOrWhiteSpace(path)
            ? 0
            : path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).Length;

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
            throw new ValidationException(parameterName + " must be greater than 0.");

        return appId;
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
            throw new ValidationException(parameterName + " is required.");

        return page;
    }

    private static IEnumerable<Page> ValidatePages(IEnumerable<Page> pages, string parameterName)
    {
        if (pages == null)
            throw new ValidationException(parameterName + " is required.");

        return pages;
    }

    private static void ValidateSinglePage(Page page, string parameterName) =>
        ThrowIf(page.Pages != null && page.Pages.Any(), "Can only import one page at a time.");

    private static void ValidatePageCollections(Page page, string parameterName)
    {
        if (page.PageInfo == null || !page.PageInfo.Any(pi => pi.CultureId == string.Empty))
            throw new ValidationException("Pages MUST have page information defined for the default culture, other cultures are optional.");

        ValidatePageLayout(page);

        if (page.Contents == null)
            throw new ValidationException("Pages MUST include a contents collection.");
    }

    private void ValidateLayoutExistsForApp(Page page)
    {
        bool layoutExists = layoutProcessingService.GetAll(ignoreFilters: true)
            .Any(layout =>
                layout.AppId == page.AppId &&
                layout.Name == page.Layout);

        if (!layoutExists)
            throw new ValidationException($"Layout '{page.Layout}' does not exist for app {page.AppId}.");
    }

    private static void ValidatePageLayout(Page page) =>
        ThrowIf(string.IsNullOrWhiteSpace(page.Layout), "Pages MUST specify a layout.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
