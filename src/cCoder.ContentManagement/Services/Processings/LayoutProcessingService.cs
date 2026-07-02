using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutProcessingService(ILayoutService service) : ILayoutProcessingService
{
    public Layout Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Layout> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<Layout> AddAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Layout> UpdateAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items)
    {
        ValidateLayouts(items, "items");
        List<Result<Layout>> results = new List<Result<Layout>>();
        foreach (Layout item in items)
        {
            try
            {
                Layout savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Layout>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Layout>
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
        ValidateLayouts(items, "items");
        foreach (Layout item in items)
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateLayout(Layout layout, string parameterName) =>
        ThrowIf(layout == null, parameterName + " is required.");

    private static void ValidateLayouts(IEnumerable<Layout> layouts, string parameterName) =>
        ThrowIf(layouts == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
