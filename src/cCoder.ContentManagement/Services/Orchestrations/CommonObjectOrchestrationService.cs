// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Rendering.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class CommonObjectOrchestrationService(
    ICommonObjectProcessingService processingService,
    ICommonObjectLatestCacheProcessingService latestCacheProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : ICommonObjectOrchestrationService
{
    public CommonObject[] DeserializeCommonObjects(object payload) =>
        TryCatch<CommonObject[]>(operation: () =>
    {
        ValidateDeserializeCommonObjects(inputs: [payload]);

        return
            processingService.DeserializeCommonObjects(
                payload: payload);
    });

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
        ValidateAllCommonObjectsOnAdd(inputs: [newCommonObjects]);

        CommonObject[] validatedCommonObjects =
        [
            .. ValidateCommonObjects(
                commonObjects: newCommonObjects,
                parameterName: "newCommonObjects")
        ];

        CommonObject[] latestCommonObjects = latestCacheProcessingService
            .GetLatestCommonObjects()
            .ToArray();

        AuthorizeImport(
            commonObjects: validatedCommonObjects,
            latestCommonObjects: latestCommonObjects);

        string userId = authorizationProcessingService.GetCurrentUserId();

        IEnumerable<OperationResult<CommonObject>> results =
            await processingService.AddAllCommonObjectsAsync(
                newCommonObjects: validatedCommonObjects,
                latestCommonObjects: latestCommonObjects,
                userId: userId);

        latestCacheProcessingService.RefreshCommonObjects(
            changedCommonObjectCount: results.Count(
                predicate: result => result.Success));

        return results;

    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "entity");
        Authorize(privilege: "commonobject_create");
        Authorize(privilege: "commonobject_update");

        CommonObject result = await processingService.UpdateCommonObjectAsync(
            updatedCommonObject: updatedCommonObject,
            userId: authorizationProcessingService.GetCurrentUserId());

        latestCacheProcessingService.RefreshCommonObjects(
            changedCommonObjectCount: 1);

        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        Authorize(privilege: "commonobject_delete");

        await processingService.DeleteAsync(commonObjectId: commonObjectId);

        latestCacheProcessingService.RefreshCommonObjects(
            changedCommonObjectCount: 1);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: () =>
    {
        ValidateOrUpdateCommonObjectResultOnAdd(inputs: [newCommonObject]);

        CommonObject[] commonObjects = ValidateCommonObjects(
            commonObjects: newCommonObject,
            parameterName: "items")
            .ToArray();

        if (commonObjects.Any(predicate: item => item.Id < 1))
        {
            Authorize(privilege: "commonobject_create");
        }

        if (commonObjects.Any(predicate: item => item.Id >= 1))
        {
            Authorize(privilege: "commonobject_create");
            Authorize(privilege: "commonobject_update");
        }

        return ExecuteAddOrUpdateCommonObjectResult(
            commonObjects: commonObjects,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);

    public ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject) =>
        TryCatch(operation: () =>
    {
        ValidateAllCommonObjectOnDelete(inputs: [deletedCommonObject]);

        CommonObject[] commonObjects = ValidateCommonObjects(
            commonObjects: deletedCommonObject,
            parameterName: "items")
            .ToArray();

        Authorize(privilege: "commonobject_delete");

        return ExecuteDeleteAllCommonObjectAsync(
            commonObjects: commonObjects);
    }, isValueTask: true);

    public IEnumerable<CommonObject> LatestCommonObject(string type) =>
        TryCatch<IEnumerable<CommonObject>>(operation: () =>
    {
        ValidateLatestCommonObject(inputs: [type]);
        ValidateType(type: type, parameterName: "type");

        return latestCacheProcessingService.GetLatestCommonObjects()
            .Where(predicate: item => item.Type == type);

    });

    private void Authorize(string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = null,
                    Privilege = privilege
                }
            });

    private async ValueTask<IEnumerable<OperationResult<CommonObject>>>
        ExecuteAddOrUpdateCommonObjectResult(
            CommonObject[] commonObjects,
            string userId)
    {
        IEnumerable<OperationResult<CommonObject>> results =
            await processingService.AddOrUpdateCommonObjectResult(
                newCommonObject: commonObjects,
                userId: userId);

        latestCacheProcessingService.RefreshCommonObjects(
            changedCommonObjectCount: results.Count(
                predicate: result => result.Success));

        return results;
    }

    private async ValueTask ExecuteDeleteAllCommonObjectAsync(
        CommonObject[] commonObjects)
    {
        await processingService.DeleteAllCommonObjectAsync(
            deletedCommonObject: commonObjects);

        latestCacheProcessingService.RefreshCommonObjects(
            changedCommonObjectCount: commonObjects.Length);
    }

    private void AuthorizeImport(
        IEnumerable<CommonObject> commonObjects,
        IEnumerable<CommonObject> latestCommonObjects)
    {
        CommonObject[] latest = latestCommonObjects.ToArray();
        bool hasAdds = false;
        bool hasUpdates = false;

        foreach (CommonObject item in commonObjects)
        {
            CommonObject existing = latest.FirstOrDefault(
                predicate: candidate =>
                    candidate.Type == item.Type
                    && candidate.Culture == (item.Culture ?? string.Empty)
                    && candidate.Name == item.Name
                    && candidate.Key == item.Key);

            hasAdds |= existing is null;

            hasUpdates |= existing is not null
                && (item.CreatedOn > existing.CreatedOn
                    || item.LastUpdated > existing.LastUpdated);
        }

        if (hasAdds || hasUpdates)
        {
            Authorize(privilege: "commonobject_create");
        }

        if (hasUpdates)
        {
            Authorize(privilege: "commonobject_update");
        }
    }

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