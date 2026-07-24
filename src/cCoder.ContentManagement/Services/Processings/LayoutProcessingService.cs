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
    public Layout Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Layout> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Layout> AddAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");
        return service.AddAsync(layout: entity);
    }

    public ValueTask<Layout> UpdateAsync(Layout entity)
    {
        ValidateLayout(layout: entity, parameterName: "entity");
        return service.UpdateAsync(layout: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items)
    {
        ValidateLayouts(layouts: items, parameterName: "items");
        List<Result<Layout>> results = new List<Result<Layout>>();

        foreach (Layout item in items)
        {
            try
            {
                Layout savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

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

    public async ValueTask DeleteAllAsync(IEnumerable<Layout> items)
    {
        ValidateLayouts(layouts: items, parameterName: "items");

        foreach (Layout item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

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
}