using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService(ICommonObjectBroker commonObjectBroker, IAuthorizationBroker authorizationBroker) : ICommonObjectService
{
    public CommonObject Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((CommonObject i) => i.Id == id);
        }

        CommonObject commonObject = GetAll().FirstOrDefault((CommonObject i) => i.Id == id);
        if (commonObject != null)
        {
            return commonObject;
        }
        CommonObject commonObject2 = GetAll(ignoreFilters: true).FirstOrDefault((CommonObject i) => i.Id == id);
        if (commonObject2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false)
    {
        return commonObjectBroker.GetAllCommonObjects(ignoreFilters);
    }

    public async ValueTask<CommonObject> AddAsync(CommonObject commonObject)
    {
        ValidateCommonObject(commonObject, "commonObject");
        CommonObject newCommonObject = CreateStorageCommonObject(commonObject);
        authorizationBroker.Authorize(commonObjectBroker.GetAppId(newCommonObject), "CommonObject_create");
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newCommonObject.CreatedOn = DateTimeOffset.UtcNow);
        newCommonObject.CreatedBy = currentUserId;
        newCommonObject.LastUpdated = now;
        newCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.AddCommonObjectAsync(newCommonObject);
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
        ValidateCommonObject(commonObject, "commonObject");
        CommonObject updateCommonObject = CreateStorageCommonObject(commonObject);
        authorizationBroker.Authorize(commonObjectBroker.GetAppId(updateCommonObject), "CommonObject_update");
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateCommonObject.LastUpdated = now;
        updateCommonObject.LastUpdatedBy = currentUserId;
        CommonObject result = await commonObjectBroker.UpdateCommonObjectAsync(updateCommonObject);
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
        ValidateId(id, "id");
        CommonObject commonObject = Get(id);
        CommonObject dataCommonObject = CreateStorageCommonObject(commonObject);
        authorizationBroker.Authorize(commonObjectBroker.GetAppId(dataCommonObject), "CommonObject_delete");
        await commonObjectBroker.DeleteCommonObjectAsync(dataCommonObject);
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

