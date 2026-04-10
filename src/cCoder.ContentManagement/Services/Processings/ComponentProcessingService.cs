using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Processings;

internal class ComponentProcessingService(IComponentService service) : IComponentProcessingService
{
    public Component Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<Component> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Component> AddAsync(Component entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<Component> UpdateAsync(Component entity)
    {
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Component>>> AddOrUpdate(IEnumerable<Component> items)
    {
        ValidateComponents(items, "items");
        List<cCoder.ContentManagement.Models.Result<Component>> results = new List<cCoder.ContentManagement.Models.Result<Component>>();
        foreach (Component item in items)
        {
            try
            {
                Component savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Component>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Component>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Component> items)
    {
        ValidateComponents(items, "items");
        foreach (Component item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateComponents(IEnumerable<Component> components, string parameterName)
    {
        if (components == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}

