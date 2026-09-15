// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CommonObjectEventOrchestrationService(
    ICommonObjectEventProcessingService eventProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
    : ICommonObjectEventOrchestrationService
{
    public ValueTask RaiseCommonObjectsImportedEventAsync(
        IEnumerable<OperationResult<CommonObject>> results) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseImported(inputs: [results]);

        CommonObject[] importedObjects = results
            .Where(predicate: result =>
                result.Success && result.Item is not null)
            .Select(selector: result => result.Item)
            .ToArray();

        if (importedObjects.Length > 0)
        {
            await eventProcessingService.RaiseCommonObjectsImportedEventAsync(
                commonObjects: importedObjects,
                userId: authorizationProcessingService.GetCurrentUserId());
        }
    }, isValueTask: true);

    public ValueTask RaiseCommonObjectUpdatedEventAsync(CommonObject commonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseUpdated(inputs: [commonObject]);

        await eventProcessingService.RaiseCommonObjectUpdateEventAsync(
            entity: commonObject,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);

    public ValueTask RaiseCommonObjectDeletedEventAsync(CommonObject commonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseDeleted(inputs: [commonObject]);

        await eventProcessingService.RaiseCommonObjectDeleteEventAsync(
            entity: commonObject,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);
}