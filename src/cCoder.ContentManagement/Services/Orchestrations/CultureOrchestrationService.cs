using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using Culture = cCoder.Data.Models.CMS.Culture;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Culture>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class CultureOrchestrationService(
    ICultureProcessingService processingService,
    ICultureEventProcessingService eventService) : ICultureOrchestrationService
{
    public Culture Get(string id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Culture> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Culture> AddAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");

        Culture result = await processingService.AddAsync(entity);
        await eventService.RaiseCultureAddEventAsync(result);
        return result;
    }

    public async ValueTask<Culture> UpdateAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");

        Culture result = await processingService.UpdateAsync(entity);
        await eventService.RaiseCultureUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        ValidateId(id, "id");

        Culture entity = processingService.Get(id);
        await eventService.RaiseCultureDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Culture> items) =>
        processingService.AddOrUpdate(ValidateCultures(items, "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Culture> items) =>
        processingService.DeleteAllAsync(ValidateCultures(items, "items"));

    private static string ValidateId(string id, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return id;
    }

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if (culture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return culture;
    }

    private static IEnumerable<Culture> ValidateCultures(IEnumerable<Culture> cultures, string parameterName)
    {
        if (cultures == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return cultures;
    }
}
