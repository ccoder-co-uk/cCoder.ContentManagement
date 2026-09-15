// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class CommonObjectCoordinationService(
    ICommonObjectOrchestrationService commonObjectOrchestrationService,
    ICommonObjectEventOrchestrationService eventOrchestrationService)
    : ICommonObjectCoordinationService
{
    public CommonObject GetCommonObject(int commonObjectId) =>
        TryCatch(operation: () =>
    {
        ValidateCommonObjectOnGet(inputs: [commonObjectId]);

        return commonObjectOrchestrationService.GetCommonObject(
            commonObjectId: commonObjectId);
    });

    public IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
    {
        ValidateAllCommonObjectOnGet(inputs: [ignoreFilters]);

        return commonObjectOrchestrationService.GetAllCommonObject(
            ignoreFilters: ignoreFilters);
    });

    public CommonObject[] DeserializeCommonObjects(object payload) =>
        TryCatch(operation: () =>
    {
        ValidateCommonObjectsOnDeserialize(inputs: [payload]);

        return commonObjectOrchestrationService.DeserializeCommonObjects(
            payload: payload);
    });

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddAllCommonObjectsAsync(
        CommonObject[] newCommonObjects) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateAllCommonObjectsOnAdd(inputs: [newCommonObjects]);

        IEnumerable<OperationResult<CommonObject>> results =
            await commonObjectOrchestrationService.AddAllCommonObjectsAsync(
                newCommonObjects: newCommonObjects);

        await eventOrchestrationService.RaiseCommonObjectsImportedEventAsync(
            results: results);

        return results;
    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);

        CommonObject result =
            await commonObjectOrchestrationService.UpdateCommonObjectAsync(
                updatedCommonObject: updatedCommonObject);

        await eventOrchestrationService.RaiseCommonObjectUpdatedEventAsync(
            commonObject: result);

        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);

        CommonObject entity = commonObjectOrchestrationService.GetCommonObject(
            commonObjectId: commonObjectId);

        await eventOrchestrationService.RaiseCommonObjectDeletedEventAsync(
            commonObject: entity);

        await commonObjectOrchestrationService.DeleteAsync(
            commonObjectId: commonObjectId);
    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>>
        AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: () =>
    {
        ValidateOrUpdateCommonObjectResultOnAdd(inputs: [newCommonObject]);

        return commonObjectOrchestrationService.AddOrUpdateCommonObjectResult(
            newCommonObject: newCommonObject);
    }, isValueTask: true);

    public ValueTask DeleteAllCommonObjectAsync(
        IEnumerable<CommonObject> deletedCommonObject) =>
        TryCatch(operation: () =>
    {
        ValidateAllCommonObjectOnDelete(inputs: [deletedCommonObject]);

        return commonObjectOrchestrationService.DeleteAllCommonObjectAsync(
            deletedCommonObject: deletedCommonObject);
    }, isValueTask: true);

    public IEnumerable<CommonObject> LatestCommonObject(string type) =>
        TryCatch(operation: () =>
    {
        ValidateLatestCommonObject(inputs: [type]);
        return commonObjectOrchestrationService.LatestCommonObject(type: type);
    });
}