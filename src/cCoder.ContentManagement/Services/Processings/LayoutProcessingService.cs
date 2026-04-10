using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutProcessingService(ILayoutService service) : ILayoutProcessingService
{
    public Layout Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Layout> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

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

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Layout>>> AddOrUpdate(IEnumerable<Layout> items)
    {
        ValidateLayouts(items, "items");
        List<cCoder.ContentManagement.Models.Result<Layout>> results = new List<cCoder.ContentManagement.Models.Result<Layout>>();
        foreach (Layout item in items)
        {
            try
            {
                Layout savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Layout>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Layout>
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
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateLayout(Layout layout, string parameterName)
    {
        if (layout == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateLayouts(IEnumerable<Layout> layouts, string parameterName)
    {
        if (layouts == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
