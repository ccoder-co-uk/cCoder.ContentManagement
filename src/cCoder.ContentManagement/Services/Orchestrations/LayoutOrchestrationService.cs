// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class LayoutOrchestrationService(
    ILayoutProcessingService processingService,
    ILayoutEventProcessingService eventService) : ILayoutOrchestrationService
{
    public Layout GetLayout(int layoutId) =>
        TryCatch<Layout>(operation: () =>
    {
        ValidateLayoutOnGet(inputs: [layoutId]);
        return processingService.GetLayout(layoutId: ValidateId(layoutId: layoutId, parameterName: "id"));
    });

    public IQueryable<Layout> GetAllLayout(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Layout>>(operation: () =>
    {
        ValidateAllLayoutOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllLayout(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Layout> AddLayoutAsync(Layout newLayout) =>
        TryCatch<Layout>(operation: async () =>
    {
        ValidateLayoutOnAdd(inputs: [newLayout]);
        ValidateLayout(layout: newLayout, parameterName: "entity");

        Layout result = await processingService.AddLayoutAsync(newLayout: newLayout);
        await eventService.RaiseLayoutAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout) =>
        TryCatch<Layout>(operation: async () =>
    {
        ValidateLayoutOnUpdate(inputs: [updatedLayout]);
        ValidateLayout(layout: updatedLayout, parameterName: "entity");

        Layout result = await processingService.UpdateLayoutAsync(updatedLayout: updatedLayout);
        await eventService.RaiseLayoutUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int layoutId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [layoutId]);
        ValidateId(layoutId: layoutId, parameterName: "id");

        Layout entity;

        try
        {
            entity = processingService.GetLayout(layoutId: layoutId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllLayout(ignoreFilters: true)
                .FirstOrDefault(predicate: layout => layout.Id == layoutId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseLayoutDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(layoutId: layoutId);

    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateByAppIdOnDelete(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        Layout[] layoutsToDelete = [.. ExecuteGetAllLayout(ignoreFilters: true)
            .Where(predicate: layout => layout.AppId == appId)];

        if (layoutsToDelete.Length > 0)
        {
            await ExecuteDeleteAllLayoutAsync(deletedLayout: layoutsToDelete);
        }

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout) =>
        TryCatch<IEnumerable<OperationResult<Layout>>>(operation: async () =>
    {
        ValidateOrUpdateLayoutResultOnAdd(inputs: [newLayout]);

        Layout[] layouts = ValidateLayouts(layouts: newLayout, parameterName: "items")
            .ToArray();

        List<OperationResult<Layout>> results = new();

        foreach (Layout layout in layouts)
        {
            try
            {
                Layout result = layout.Id <= 0
                    ? await ExecuteAddLayoutAsync(newLayout: layout)
                    : await ExecuteUpdateLayoutAsync(updatedLayout: layout);

                results.Add(item: new OperationResult<Layout>
                {
                    Success = true,
                    Item = result,
                    Message = layout.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Layout>
                {
                    Success = false,
                    Item = layout,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask ImportLayoutsAsync(int appId, Layout[] items) =>
        TryCatch(operation: async () =>
    {
        ValidateImportLayoutsAsync(inputs: [appId, items]);
        ValidateAppId(appId: appId, parameterName: "appId");

        Layout[] validatedItems = ValidateLayouts(layouts: items, parameterName: "items")
            .ToArray();

        string[] names = validatedItems.Select(selector: layout => layout.Name.ToLower())
            .ToArray();

        var dbVersions = processingService.GetAllLayout()
            .Where(predicate: layout => layout.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: layout.Name.ToLower()))
            .Select(selector: layout => new { layout.Id, layout.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: layout =>
        {
            layout.AppId = appId;

            layout.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: layout.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await processingService.AddOrUpdateLayoutResult(
            newLayout: validatedItems);

    }, isValueTask: true);

    public ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout) =>
        TryCatch(operation: async () =>
    {
        ValidateAllLayoutOnDelete(inputs: [deletedLayout]);

        Layout[] layouts = ValidateLayouts(layouts: deletedLayout, parameterName: "items")
            .ToArray();

        foreach (Layout layout in layouts)
        {
            await ExecuteDeleteAsync(layoutId: layout.Id);
        }

    }, isValueTask: true);

    private static int ValidateId(int layoutId, string parameterName)
    {
        if (layoutId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return layoutId;
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

    private async ValueTask<Layout> ExecuteAddLayoutAsync(Layout newLayout)
    {
        ValidateLayout(layout: newLayout, parameterName: "entity");

        Layout result = await processingService.AddLayoutAsync(newLayout: newLayout);
        await eventService.RaiseLayoutAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask<IEnumerable<OperationResult<Layout>>> ExecuteAddOrUpdateLayoutResult(IEnumerable<Layout> newLayout)
    {
        Layout[] layouts = ValidateLayouts(layouts: newLayout, parameterName: "items")
            .ToArray();

        List<OperationResult<Layout>> results = new();

        foreach (Layout layout in layouts)
        {
            try
            {
                Layout result = layout.Id <= 0
                    ? await ExecuteAddLayoutAsync(newLayout: layout)
                    : await ExecuteUpdateLayoutAsync(updatedLayout: layout);

                results.Add(item: new OperationResult<Layout>
                {
                    Success = true,
                    Item = result,
                    Message = layout.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Layout>
                {
                    Success = false,
                    Item = layout,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private async ValueTask ExecuteDeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout)
    {
        Layout[] layouts = ValidateLayouts(layouts: deletedLayout, parameterName: "items")
            .ToArray();

        foreach (Layout layout in layouts)
        {
            await ExecuteDeleteAsync(layoutId: layout.Id);
        }
    }

    private async ValueTask ExecuteDeleteAsync(int layoutId)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");

        Layout entity;

        try
        {
            entity = processingService.GetLayout(layoutId: layoutId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllLayout(ignoreFilters: true)
                .FirstOrDefault(predicate: layout => layout.Id == layoutId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseLayoutDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(layoutId: layoutId);
    }

    private IQueryable<Layout> ExecuteGetAllLayout(bool ignoreFilters = false) =>
        processingService.GetAllLayout(ignoreFilters: ignoreFilters);

    private async ValueTask<Layout> ExecuteUpdateLayoutAsync(Layout updatedLayout)
    {
        ValidateLayout(layout: updatedLayout, parameterName: "entity");

        Layout result = await processingService.UpdateLayoutAsync(updatedLayout: updatedLayout);
        await eventService.RaiseLayoutUpdateEventAsync(entity: result);
        return result;
    }
}