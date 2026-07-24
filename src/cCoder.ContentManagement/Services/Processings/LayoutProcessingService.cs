// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutProcessingService(ILayoutService service) : ILayoutProcessingService
{
    public Layout GetLayout(int layoutId)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");
        return service.GetLayout(layoutId: layoutId);
    }

    public IQueryable<Layout> GetAllLayout(bool ignoreFilters = false) =>
        service.GetAllLayout(ignoreFilters: ignoreFilters);

    public ValueTask<Layout> AddLayoutAsync(Layout newLayout)
    {
        ValidateLayout(layout: newLayout, parameterName: "entity");
        return service.AddLayoutAsync(newLayout: newLayout);
    }

    public ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout)
    {
        ValidateLayout(layout: updatedLayout, parameterName: "entity");
        return service.UpdateLayoutAsync(updatedLayout: updatedLayout);
    }

    public ValueTask DeleteAsync(int layoutId)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");
        return service.DeleteAsync(layoutId: layoutId);
    }

    public async ValueTask<IEnumerable<Result<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout)
    {
        ValidateLayouts(layouts: newLayout, parameterName: "items");
        List<Result<Layout>> results = new List<Result<Layout>>();

        foreach (Layout item in newLayout)
        {
            try
            {
                Layout savedItem = item.Id < 1 ? await ExecuteAddLayoutAsync(newLayout: item) : await ExecuteUpdateLayoutAsync(updatedLayout: item);

                results.Add(item: new Result<Layout>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Layout>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout)
    {
        ValidateLayouts(layouts: deletedLayout, parameterName: "items");

        foreach (Layout item in deletedLayout)
        {
            await ExecuteDeleteAsync(layoutId: item.Id);
        }
    }

    private static void ValidateId(int layoutId, string parameterName) =>
        ThrowIf(condition: layoutId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateLayout(Layout layout, string parameterName) =>
        ThrowIf(condition: layout == null, message: parameterName + " is required.");

    private static void ValidateLayouts(IEnumerable<Layout> layouts, string parameterName) =>
        ThrowIf(condition: layouts == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private ValueTask<Layout> ExecuteAddLayoutAsync(Layout newLayout)
    {
        ValidateLayout(layout: newLayout, parameterName: "entity");
        return service.AddLayoutAsync(newLayout: newLayout);
    }

    private ValueTask ExecuteDeleteAsync(int layoutId)
    {
        ValidateId(layoutId: layoutId, parameterName: "id");
        return service.DeleteAsync(layoutId: layoutId);
    }

    private ValueTask<Layout> ExecuteUpdateLayoutAsync(Layout updatedLayout)
    {
        ValidateLayout(layout: updatedLayout, parameterName: "entity");
        return service.UpdateLayoutAsync(updatedLayout: updatedLayout);
    }
}