// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService(ICommonObjectBroker commonObjectBroker, IAuthorizationManager authorizationManager) : ICommonObjectService
{
    public CommonObject GetCommonObject(int commonObjectId, bool ignoreFilters = false) =>
        TryCatch<CommonObject>(operation: () =>
    {
        ValidateCommonObjectOnGet(inputs: [commonObjectId, ignoreFilters]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllCommonObject(ignoreFilters: true)
                .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);
        }

        CommonObject commonObject = ExecuteGetAllCommonObject()
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);

        if (commonObject != null)
        {
            return commonObject;
        }

        CommonObject commonObject2 = ExecuteGetAllCommonObject(ignoreFilters: true)
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);

        if (commonObject2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false) =>
        TryCatch<IQueryable<CommonObject>>(operation: () =>
    {
        ValidateAllCommonObjectOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? commonObjectBroker.GetAllCommonObjectsIgnoringFilters()
            : commonObjectBroker.GetAllCommonObjects();
    });

    public ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnAdd(inputs: [newCommonObject]);
        ValidateCommonObject(commonObject: newCommonObject, parameterName: "commonObject");
        CommonObject storageCommonObject = CreateStorageCommonObject(newCommonObject: newCommonObject);
        authorizationManager.Authorize(appId: commonObjectBroker.GetAppId(entity: storageCommonObject), privilege: "CommonObject_create");

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageCommonObject.CreatedOn = DateTimeOffset.UtcNow);
        storageCommonObject.CreatedBy = currentUserId;
        storageCommonObject.LastUpdated = now;
        storageCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.AddCommonObjectAsync(newCommonObject: storageCommonObject);
        newCommonObject.Id = result.Id;
        newCommonObject.Name = result.Name;
        newCommonObject.Description = result.Description;
        newCommonObject.LastUpdated = result.LastUpdated;
        newCommonObject.LastUpdatedBy = result.LastUpdatedBy;
        newCommonObject.CreatedOn = result.CreatedOn;
        newCommonObject.CreatedBy = result.CreatedBy;
        newCommonObject.Version = result.Version;
        newCommonObject.Key = result.Key;
        newCommonObject.Type = result.Type;
        newCommonObject.Json = result.Json;
        newCommonObject.Culture = result.Culture;
        return newCommonObject;

    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "commonObject");
        CommonObject updateCommonObject = CreateStorageCommonObject(newCommonObject: updatedCommonObject);
        authorizationManager.Authorize(appId: commonObjectBroker.GetAppId(entity: updateCommonObject), privilege: "CommonObject_update");

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateCommonObject.LastUpdated = now;
        updateCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.UpdateCommonObjectAsync(updatedCommonObject: updateCommonObject);
        updatedCommonObject.Id = result.Id;
        updatedCommonObject.Name = result.Name;
        updatedCommonObject.Description = result.Description;
        updatedCommonObject.LastUpdated = result.LastUpdated;
        updatedCommonObject.LastUpdatedBy = result.LastUpdatedBy;
        updatedCommonObject.CreatedOn = result.CreatedOn;
        updatedCommonObject.CreatedBy = result.CreatedBy;
        updatedCommonObject.Version = result.Version;
        updatedCommonObject.Key = result.Key;
        updatedCommonObject.Type = result.Type;
        updatedCommonObject.Json = result.Json;
        updatedCommonObject.Culture = result.Culture;
        return updatedCommonObject;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        CommonObject commonObject = ExecuteGetCommonObject(commonObjectId: commonObjectId);
        CommonObject dataCommonObject = CreateStorageCommonObject(newCommonObject: commonObject);
        authorizationManager.Authorize(appId: commonObjectBroker.GetAppId(entity: dataCommonObject), privilege: "CommonObject_delete");
        await commonObjectBroker.DeleteCommonObjectAsync(deletedCommonObject: dataCommonObject);

    }, isValueTask: true);

    private static CommonObject CreateStorageCommonObject(CommonObject newCommonObject)
    {
        if (newCommonObject == null)
        {
            return null;
        }

        return new CommonObject
        {
            Id = newCommonObject.Id,
            Name = newCommonObject.Name,
            Description = newCommonObject.Description,
            LastUpdated = newCommonObject.LastUpdated,
            LastUpdatedBy = newCommonObject.LastUpdatedBy,
            CreatedOn = newCommonObject.CreatedOn,
            CreatedBy = newCommonObject.CreatedBy,
            Version = newCommonObject.Version,
            Key = newCommonObject.Key,
            Type = newCommonObject.Type,
            Json = newCommonObject.Json,
            Culture = newCommonObject.Culture
        };
    }

    private IQueryable<CommonObject> ExecuteGetAllCommonObject(bool ignoreFilters = false) =>
        (ignoreFilters
            ? commonObjectBroker.GetAllCommonObjectsIgnoringFilters()
            : commonObjectBroker.GetAllCommonObjects());

    private CommonObject ExecuteGetCommonObject(int commonObjectId, bool ignoreFilters = false)
    {
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllCommonObject(ignoreFilters: true)
                .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);
        }

        CommonObject commonObject = ExecuteGetAllCommonObject()
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);

        if (commonObject != null)
        {
            return commonObject;
        }

        CommonObject commonObject2 = ExecuteGetAllCommonObject(ignoreFilters: true)
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == commonObjectId);

        if (commonObject2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}