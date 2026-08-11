// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class CommonObjectOrchestrationService(ICommonObjectProcessingService processingService, ICommonObjectEventProcessingService eventService) : ICommonObjectOrchestrationService
{
    public CommonObject GetCommonObject(int commonObjectId) =>
        TryCatch<CommonObject>(operation: () =>
    {
        ValidateCommonObjectOnGet(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        return processingService.GetCommonObject(commonObjectId: commonObjectId);

    });

    public IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false) =>
        TryCatch<IQueryable<CommonObject>>(operation: () =>
    {
        ValidateAllCommonObjectOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllCommonObject(ignoreFilters: ignoreFilters);
    });

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(
        CommonObject[] newCommonObjects) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateImportCommonObjectResultAsync(inputs: [newCommonObjects]);

        CommonObject[] validatedCommonObjects =
        [
            .. ValidateCommonObjects(
                commonObjects: newCommonObjects,
                parameterName: "newCommonObjects")
        ];

        IEnumerable<OperationResult<CommonObject>> results =
            await processingService.AddAllCommonObjectsAsync(
                newCommonObjects: validatedCommonObjects);

        CommonObject[] importedObjects =
        [
            .. results
                .Where(predicate: result =>
                    result.Success && result.Item is not null)
                .Select(selector: result => result.Item)
        ];

        if (importedObjects.Length > 0)
        {
            await eventService.RaiseCommonObjectsImportedEventAsync(
                commonObjects: importedObjects);
        }

        return results;

    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "entity");
        CommonObject result = await processingService.UpdateCommonObjectAsync(updatedCommonObject: updatedCommonObject);
        await eventService.RaiseCommonObjectUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        CommonObject entity = processingService.GetCommonObject(commonObjectId: commonObjectId);
        await eventService.RaiseCommonObjectDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(commonObjectId: commonObjectId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: () =>
    {
        ValidateOrUpdateCommonObjectResultOnAdd(inputs: [newCommonObject]);
        return processingService.AddOrUpdateCommonObjectResult(newCommonObject: ValidateCommonObjects(commonObjects: newCommonObject, parameterName: "items"));
    }, isValueTask: true);

    public ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject) =>
        TryCatch(operation: () =>
    {
        ValidateAllCommonObjectOnDelete(inputs: [deletedCommonObject]);
        return processingService.DeleteAllCommonObjectAsync(deletedCommonObject: ValidateCommonObjects(commonObjects: deletedCommonObject, parameterName: "items"));
    }, isValueTask: true);

    public IEnumerable<CommonObject> LatestCommonObject(string type) =>
        TryCatch<IEnumerable<CommonObject>>(operation: () =>
    {
        ValidateLatestCommonObject(inputs: [type]);
        ValidateType(type: type, parameterName: "type");
        return processingService.LatestCommonObject(type: type);

    });

    private static void ValidateId(int commonObjectId, string parameterName) =>
        ThrowIf(condition: commonObjectId < 1, message: parameterName + " must be greater than 0.");

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