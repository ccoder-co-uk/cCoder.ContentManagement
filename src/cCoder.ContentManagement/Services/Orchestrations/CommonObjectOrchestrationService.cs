using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class CommonObjectOrchestrationService(ICommonObjectProcessingService processingService, ICommonObjectEventProcessingService eventService) : ICommonObjectOrchestrationService
{
    public CommonObject Get(int id)
    {
        ValidateId(id, "id");
        return processingService.Get(id);
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<CommonObject> AddAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");
        CommonObject result = await processingService.AddAsync(entity);
        await eventService.RaiseCommonObjectAddEventAsync(result);
        return result;
    }

    public async ValueTask<CommonObject> UpdateAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");
        CommonObject result = await processingService.UpdateAsync(entity);
        await eventService.RaiseCommonObjectUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        CommonObject entity = processingService.Get(id);
        await eventService.RaiseCommonObjectDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<CommonObject>>> AddOrUpdate(IEnumerable<CommonObject> items) =>
        processingService.AddOrUpdate(ValidateCommonObjects(items, "items"));

    public ValueTask DeleteAllAsync(IEnumerable<CommonObject> items) =>
        processingService.DeleteAllAsync(ValidateCommonObjects(items, "items"));

    public IEnumerable<CommonObject> Latest(string type)
    {
        ValidateType(type, "type");
        return processingService.Latest(type);
    }

    public ValueTask<IEnumerable<Result<CommonObject>>> ImportAsync(IEnumerable<CommonObject> items) =>
        processingService.ImportAsync(ValidateCommonObjects(items, "items"));

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateType(string type, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(type), parameterName + " is required.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(commonObject == null, parameterName + " is required.");

    private static IEnumerable<CommonObject> ValidateCommonObjects(IEnumerable<CommonObject> commonObjects, string parameterName)
    {
        if (commonObjects == null)
            throw new ValidationException(parameterName + " is required.");

        return commonObjects;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
