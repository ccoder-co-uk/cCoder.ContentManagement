using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureProcessingService(ICultureService service) : ICultureProcessingService
{
    public Culture Get(string id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Culture> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

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

    public async ValueTask<IEnumerable<Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items)
    {
        ValidateCultures(items, "items");
        List<Result<Culture>> results = new List<Result<Culture>>();
        foreach (Culture item in items)
        {
            try
            {
                Culture savedItem = string.IsNullOrWhiteSpace(item.Id) ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Culture>
                {
                    Success = true,
                    Item = savedItem,
                    Message = string.IsNullOrWhiteSpace(item.Id) ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Culture>
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
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(string id, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(id), parameterName + " is required.");

    private static void ValidateCulture(Culture culture, string parameterName) =>
        ThrowIf((object)culture == null, parameterName + " is required.");

    private static void ValidateCultures(IEnumerable<Culture> cultures, string parameterName) =>
        ThrowIf(cultures == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
