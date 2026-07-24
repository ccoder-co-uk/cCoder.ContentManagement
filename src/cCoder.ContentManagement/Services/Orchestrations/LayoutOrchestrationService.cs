// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class LayoutOrchestrationService(
    ILayoutProcessingService processingService,
    ILayoutEventProcessingService eventService) : ILayoutOrchestrationService
{
    public Layout Get(int id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Layout> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Layout> AddAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");

        Layout result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseLayoutAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Layout> UpdateAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");

        Layout result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseLayoutUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Layout entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: layout => layout.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseLayoutDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Layout[] layoutsToDelete = [.. GetAll(ignoreFilters: true)
            .Where(predicate: layout => layout.AppId == appId)];

        if (layoutsToDelete.Length > 0)
        {
            await DeleteAllAsync(items: layoutsToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items)
    {
        Layout[] layouts = ValidateLayouts(layouts: items, parameterName: "items")
            .ToArray();

        List<Result<Layout>> results = new();

        foreach (Layout layout in layouts)
        {
            try
            {
                Layout result = layout.Id <= 0
                    ? await AddAsync(entity: layout)
                    : await UpdateAsync(entity: layout);

                results.Add(item: new Result<Layout>
                {
                    Success = true,
                    Item = result,
                    Message = layout.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Layout>
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
        ValidateAppId(appId: appId, parameterName: "appId");

        Layout[] validatedItems = ValidateLayouts(layouts: items, parameterName: "items")
            .ToArray();

        string[] names = validatedItems.Select(selector: layout => layout.Name.ToLower())
            .ToArray();

        var dbVersions = processingService.GetAll()
            .Where(predicate: layout => layout.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: layout.Name.ToLower()))
            .Select(selector: layout => new { layout.Id, layout.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: layout =>
        {
            layout.AppId = appId;

            layout.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: layout.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(items: validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Layout> items)
    {
        Layout[] layouts = ValidateLayouts(layouts: items, parameterName: "items")
            .ToArray();

        foreach (Layout layout in layouts)
        {
            await DeleteAsync(id: layout.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return id;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Layout ValidateLayout(Layout layout, string parameterName)
    {
        if (layout == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return layout;
    }

    private static IEnumerable<Layout> ValidateLayouts(IEnumerable<Layout> layouts, string parameterName)
    {
        if (layouts == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return layouts;
    }
}