// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService(ICommonObjectBroker commonObjectBroker, IAuthorizationBroker authorizationBroker) : ICommonObjectService
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
        return commonObjectBroker.GetAllCommonObjects(ignoreFilters: ignoreFilters);
    });

    public ValueTask<CommonObject> AddCommonObjectAsync(CommonObject commonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnAdd(inputs: [commonObject]);
        ValidateCommonObject(commonObject: commonObject, parameterName: "commonObject");
        CommonObject newCommonObject = CreateStorageCommonObject(newCommonObject: commonObject);
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: newCommonObject), privilege: "CommonObject_create");

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newCommonObject.CreatedOn = DateTimeOffset.UtcNow);
        newCommonObject.CreatedBy = currentUserId;
        newCommonObject.LastUpdated = now;
        newCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.AddCommonObjectAsync(newCommonObject: newCommonObject);
        commonObject.Id = result.Id;
        commonObject.Name = result.Name;
        commonObject.Description = result.Description;
        commonObject.LastUpdated = result.LastUpdated;
        commonObject.LastUpdatedBy = result.LastUpdatedBy;
        commonObject.CreatedOn = result.CreatedOn;
        commonObject.CreatedBy = result.CreatedBy;
        commonObject.Version = result.Version;
        commonObject.Key = result.Key;
        commonObject.Type = result.Type;
        commonObject.Json = result.Json;
        commonObject.Culture = result.Culture;
        return commonObject;

    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "commonObject");
        CommonObject updateCommonObject = CreateStorageCommonObject(newCommonObject: updatedCommonObject);
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: updateCommonObject), privilege: "CommonObject_update");

        string currentUserId = authorizationBroker.GetCurrentUser()
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
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: dataCommonObject), privilege: "CommonObject_delete");
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
        commonObjectBroker.GetAllCommonObjects(ignoreFilters: ignoreFilters);

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