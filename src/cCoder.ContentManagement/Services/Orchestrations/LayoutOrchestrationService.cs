using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Processings;
using Layout = cCoder.Data.Models.CMS.Layout;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Layout>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class LayoutOrchestrationService(
    ILayoutProcessingService processingService,
    ILayoutEventProcessingService eventService) : ILayoutOrchestrationService
{
    public Layout Get(int id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Layout> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Layout> AddAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");

        Layout result = await processingService.AddAsync(entity);
        await eventService.RaiseLayoutAddEventAsync(result);
        return result;
    }

    public async ValueTask<Layout> UpdateAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");

        Layout result = await processingService.UpdateAsync(entity);
        await eventService.RaiseLayoutUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Layout entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(layout => layout.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseLayoutDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Layout[] layoutsToDelete = [.. GetAll(ignoreFilters: true).Where(layout => layout.AppId == appId)];

        if (layoutsToDelete.Length > 0)
        {
            await DeleteAllAsync(layoutsToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Layout> items)
    {
        Layout[] layouts = ValidateLayouts(items, "items").ToArray();
        List<Result> results = new();

        foreach (Layout layout in layouts)
        {
            try
            {
                Layout result = layout.Id <= 0
                    ? await AddAsync(layout)
                    : await UpdateAsync(layout);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = layout.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
                {
                    Success = false,
                    Item = layout,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportLayoutsAsync(int appId, Layout[] items)
    {
        ValidateAppId(appId, "appId");

        Layout[] validatedItems = ValidateLayouts(items, "items").ToArray();
        string[] names = validatedItems.Select(layout => layout.Name.ToLower()).ToArray();

        var dbVersions = (from layout in processingService.GetAll()
                          where layout.AppId == appId && ((ReadOnlySpan<string>)names).Contains(layout.Name.ToLower())
                          select new { layout.Id, layout.Name }).ToArray();

        Array.ForEach(validatedItems, layout =>
        {
            layout.AppId = appId;
            layout.Id = dbVersions.FirstOrDefault(existing =>
                existing.Name.Equals(layout.Name, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Layout> items)
    {
        Layout[] layouts = ValidateLayouts(items, "items").ToArray();

        foreach (Layout layout in layouts)
        {
            await DeleteAsync(layout.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return id;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Layout ValidateLayout(Layout layout, string parameterName)
    {
        if (layout == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return layout;
    }

    private static IEnumerable<Layout> ValidateLayouts(IEnumerable<Layout> layouts, string parameterName)
    {
        if (layouts == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return layouts;
    }
}
