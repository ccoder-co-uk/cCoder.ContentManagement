using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using CommonObject = cCoder.Data.Models.CommonObject;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CommonObject>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class CommonObjectOrchestrationService(ICommonObjectProcessingService processingService, ICommonObjectEventProcessingService eventService) : ICommonObjectOrchestrationService
{
    public CommonObject Get(int id)
    {
        ValidateId(id, "id");
        return processingService.Get(id);
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

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

    public ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<CommonObject> items)
    {
        return processingService.AddOrUpdate(ValidateCommonObjects(items, "items"));
    }

    public ValueTask DeleteAllAsync(IEnumerable<CommonObject> items)
    {
        return processingService.DeleteAllAsync(ValidateCommonObjects(items, "items"));
    }

    public IEnumerable<CommonObject> Latest(string type)
    {
        ValidateType(type, "type");
        return processingService.Latest(type);
    }

    public ValueTask<IEnumerable<Result>> ImportAsync(IEnumerable<CommonObject> items)
    {
        return processingService.ImportAsync(ValidateCommonObjects(items, "items"));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateType(string type, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static IEnumerable<CommonObject> ValidateCommonObjects(IEnumerable<CommonObject> commonObjects, string parameterName)
    {
        if (commonObjects == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return commonObjects;
    }
}
