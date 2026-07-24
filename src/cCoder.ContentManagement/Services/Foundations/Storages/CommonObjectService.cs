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
    public CommonObject Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (CommonObject i) => i.Id == id);
        }

        CommonObject commonObject = GetAll()
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == id);

        if (commonObject != null)
        {
            return commonObject;
        }

        CommonObject commonObject2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (CommonObject i) => i.Id == id);

        if (commonObject2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false) =>
        commonObjectBroker.GetAllCommonObjects(ignoreFilters: ignoreFilters);

    public async ValueTask<CommonObject> AddAsync(CommonObject commonObject)
    {
        ValidateCommonObject(commonObject: commonObject, parameterName: "commonObject");
        CommonObject newCommonObject = CreateStorageCommonObject(commonObject: commonObject);
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: newCommonObject), privilege: "CommonObject_create");

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newCommonObject.CreatedOn = DateTimeOffset.UtcNow);
        newCommonObject.CreatedBy = currentUserId;
        newCommonObject.LastUpdated = now;
        newCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.AddCommonObjectAsync(entity: newCommonObject);
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
    }

    public async ValueTask<CommonObject> UpdateAsync(CommonObject commonObject)
    {
        ValidateCommonObject(commonObject: commonObject, parameterName: "commonObject");
        CommonObject updateCommonObject = CreateStorageCommonObject(commonObject: commonObject);
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: updateCommonObject), privilege: "CommonObject_update");

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateCommonObject.LastUpdated = now;
        updateCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.UpdateCommonObjectAsync(entity: updateCommonObject);
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
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        CommonObject commonObject = Get(id: id);
        CommonObject dataCommonObject = CreateStorageCommonObject(commonObject: commonObject);
        authorizationBroker.Authorize(appId: commonObjectBroker.GetAppId(entity: dataCommonObject), privilege: "CommonObject_delete");
        await commonObjectBroker.DeleteCommonObjectAsync(entity: dataCommonObject);
    }

    private static CommonObject CreateStorageCommonObject(CommonObject commonObject)
    {
        if (commonObject == null)
        {
            return null;
        }

        return new CommonObject
        {
            Id = commonObject.Id,
            Name = commonObject.Name,
            Description = commonObject.Description,
            LastUpdated = commonObject.LastUpdated,
            LastUpdatedBy = commonObject.LastUpdatedBy,
            CreatedOn = commonObject.CreatedOn,
            CreatedBy = commonObject.CreatedBy,
            Version = commonObject.Version,
            Key = commonObject.Key,
            Type = commonObject.Type,
            Json = commonObject.Json,
            Culture = commonObject.Culture
        };
    }
}