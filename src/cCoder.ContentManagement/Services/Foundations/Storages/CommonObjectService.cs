// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService(
    ICommonObjectBroker commonObjectBroker,
    IJsonBroker jsonBroker) : ICommonObjectService
{
    public CommonObject[] DeserializeCommonObjects(object payload) =>
        TryCatch<CommonObject[]>(operation: () =>
    {
        ValidateCommonObjectsOnDeserialize(inputs: [payload]);
        JsonRecordsDocument document = jsonBroker.ParseRecords(payload: payload);

        return document?.Records
            .Select(selector: record =>
                jsonBroker.ParseJson<CommonObject>(json: record.RawText))
            .ToArray();
    });

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

    public ValueTask<CommonObject> AddCommonObjectAsync(
        CommonObject newCommonObject,
        string userId) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnAdd(inputs: [newCommonObject, userId]);
        ValidateCommonObject(commonObject: newCommonObject, parameterName: "commonObject");
        CommonObject storageCommonObject = CreateStorageCommonObject(newCommonObject: newCommonObject);
        DateTimeOffset now = (storageCommonObject.CreatedOn = DateTimeOffset.UtcNow);
        storageCommonObject.CreatedBy = userId;
        storageCommonObject.LastUpdated = now;
        storageCommonObject.LastUpdatedBy = userId;
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

    public ValueTask<CommonObject> UpdateCommonObjectAsync(
        CommonObject updatedCommonObject,
        string userId) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject, userId]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "commonObject");
        CommonObject updateCommonObject = CreateStorageCommonObject(newCommonObject: updatedCommonObject);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateCommonObject.LastUpdated = now;
        updateCommonObject.LastUpdatedBy = userId;
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