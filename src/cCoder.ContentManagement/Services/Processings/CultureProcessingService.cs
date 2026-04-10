using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureProcessingService(ICultureService service) : ICultureProcessingService
{
    public Culture Get(string id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Culture> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Culture> AddAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Culture> UpdateAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(string id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items)
    {
        ValidateCultures(items, "items");
        List<cCoder.ContentManagement.Models.Result<Culture>> results = new List<cCoder.ContentManagement.Models.Result<Culture>>();
        foreach (Culture item in items)
        {
            try
            {
                Culture savedItem = string.IsNullOrWhiteSpace(item.Id) ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Culture>
                {
                    Success = true,
                    Item = savedItem,
                    Message = string.IsNullOrWhiteSpace(item.Id) ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Culture>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Culture> items)
    {
        ValidateCultures(items, "items");
        foreach (Culture item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(string id, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCultures(IEnumerable<Culture> cultures, string parameterName)
    {
        if (cultures == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
