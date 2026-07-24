// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class CommonObjectOrchestrationService(ICommonObjectProcessingService processingService, ICommonObjectEventProcessingService eventService) : ICommonObjectOrchestrationService
{
    public CommonObject Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.Get(id: id);
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<CommonObject> AddAsync(CommonObject entity)
    {
        ValidateCommonObject(commonObject: entity, parameterName: "entity");
        CommonObject result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseCommonObjectAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<CommonObject> UpdateAsync(CommonObject entity)
    {
        ValidateCommonObject(commonObject: entity, parameterName: "entity");
        CommonObject result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseCommonObjectUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        CommonObject entity = processingService.Get(id: id);
        await eventService.RaiseCommonObjectDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<CommonObject>>> AddOrUpdate(IEnumerable<CommonObject> items) =>
        processingService.AddOrUpdate(items: ValidateCommonObjects(commonObjects: items, parameterName: "items"));

    public ValueTask DeleteAllAsync(IEnumerable<CommonObject> items) =>
        processingService.DeleteAllAsync(items: ValidateCommonObjects(commonObjects: items, parameterName: "items"));

    public IEnumerable<CommonObject> Latest(string type)
    {
        ValidateType(type: type, parameterName: "type");
        return processingService.Latest(type: type);
    }

    public ValueTask<IEnumerable<Result<CommonObject>>> ImportAsync(IEnumerable<CommonObject> items) =>
        processingService.ImportAsync(items: ValidateCommonObjects(commonObjects: items, parameterName: "items"));

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateType(string type, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: type), message: parameterName + " is required.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(condition: commonObject == null, message: parameterName + " is required.");

    private static IEnumerable<CommonObject> ValidateCommonObjects(IEnumerable<CommonObject> commonObjects, string parameterName)
    {
        if (commonObjects == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return commonObjects;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}