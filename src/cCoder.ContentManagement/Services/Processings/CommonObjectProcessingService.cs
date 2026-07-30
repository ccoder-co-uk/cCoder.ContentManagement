// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CommonObjectProcessingService(ICommonObjectService service, ICommonObjectReaderBroker cache, IAuthorizationManager authorizationManager, IJsonBroker jsonBroker) : ICommonObjectProcessingService
{
    private User GetCurrentUser() =>
        authorizationManager.GetCurrentUser();

    public CommonObject GetCommonObject(int commonObjectId) =>
        TryCatch<CommonObject>(operation: () =>
    {
        ValidateCommonObjectOnGet(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        return service.GetCommonObject(commonObjectId: commonObjectId);

    });

    public IQueryable<CommonObject> GetAllCommonObject(bool ignoreFilters = false) =>
        TryCatch<IQueryable<CommonObject>>(operation: () =>
    {
        ValidateAllCommonObjectOnGet(inputs: [ignoreFilters]);
        return service.GetAllCommonObject(ignoreFilters: ignoreFilters);
    });

    public IEnumerable<CommonObject> LatestCommonObject(string type) =>
        TryCatch<IEnumerable<CommonObject>>(operation: () =>
    {
        ValidateLatestCommonObject(inputs: [type]);
        ValidateType(type: type, parameterName: "type");

        return cache.GetLatestSet()
            .Where(predicate: item => item.Type == type);

    });

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> ImportCommonObjectResultAsync(IEnumerable<CommonObject> items) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateImportCommonObjectResultAsync(inputs: [items]);
        ValidateCommonObjects(commonObjects: items, parameterName: "items");
        CommonObject[] commonObjects = (items as CommonObject[]) ?? items.ToArray();

        IEnumerable<string> types = commonObjects.Select(selector: (CommonObject i) => i.Type)
            .Distinct();

        List<OperationResult<CommonObject>> results = new List<OperationResult<CommonObject>>();
        List<CommonObject> adds = new List<CommonObject>();
        List<CommonObject> updates = new List<CommonObject>();

        foreach (string type in types)
        {
            IEnumerable<CommonObject> dbSet = ExecuteLatestCommonObject(type: type);

            CommonObject[] newSet = commonObjects.Where(predicate: (CommonObject i) => i.Type == type)
                .ToArray();

            CommonObject[] array = newSet;

            foreach (CommonObject entry in array)
            {
                CommonObject matchedDbEntry = dbSet.FirstOrDefault(predicate: (CommonObject dbc) => MatchesOnCultureNameAndKey(dbc: dbc, commonObject: entry));

                if (matchedDbEntry == null)
                {
                    entry.Id = 0;
                    entry.Version = 1;
                    adds.Add(item: entry);
                }
                else
                {
                    if (entry.CreatedOn > matchedDbEntry.CreatedOn || entry.LastUpdated > matchedDbEntry.LastUpdated)
                    {
                        entry.Version = matchedDbEntry.Version + 1;
                        updates.Add(item: entry);
                    }
                }
            }
        }

        results.AddRange(collection: await ExecuteAddOrUpdateCommonObjectResult(newCommonObject: adds));
        results.AddRange(collection: await ExecuteAddOrUpdateCommonObjectResult(newCommonObject: updates));
        return results;

        static bool MatchesOnCultureNameAndKey(CommonObject dbc, CommonObject commonObject)
        {
            return dbc.Culture == commonObject.Culture && dbc.Name == commonObject.Name && dbc.Key == commonObject.Key;
        }

    }, isValueTask: true);

    public ValueTask<CommonObject> AddCommonObjectAsync(CommonObject newCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnAdd(inputs: [newCommonObject]);
        ValidateCommonObject(commonObject: newCommonObject, parameterName: "entity");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_create");
        return await service.AddCommonObjectAsync(newCommonObject: newCommonObject);

    }, isValueTask: true);

    public ValueTask<CommonObject> UpdateCommonObjectAsync(CommonObject updatedCommonObject) =>
        TryCatch<CommonObject>(operation: async () =>
    {
        ValidateCommonObjectOnUpdate(inputs: [updatedCommonObject]);
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "entity");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_create");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_update");

        int newVersionCount = service.GetAllCommonObject()
            .Count(predicate: (CommonObject c) => c.Name == updatedCommonObject.Name && c.Type == updatedCommonObject.Type && c.Culture == updatedCommonObject.Culture && c.Key == updatedCommonObject.Key) + 1;

        int newVersionFromField = service.GetAllCommonObject()
            .Where(predicate: item => item.Name == updatedCommonObject.Name && item.Type == updatedCommonObject.Type && item.Culture == updatedCommonObject.Culture && item.Key == updatedCommonObject.Key)
            .OrderByDescending(keySelector: item => item.Version)
            .FirstOrDefault()?.Version ?? 1;

        updatedCommonObject.Id = 0;
        updatedCommonObject.Version = ((newVersionCount > newVersionFromField) ? newVersionCount : (newVersionFromField + 1));
        updatedCommonObject.CreatedOn = DateTimeOffset.Now;
        updatedCommonObject.LastUpdated = DateTimeOffset.Now;
        updatedCommonObject.LastUpdatedBy = GetCurrentUser().Id;
        updatedCommonObject.CreatedBy = GetCurrentUser().Id;
        updatedCommonObject = await service.AddCommonObjectAsync(newCommonObject: updatedCommonObject);

        if (updatedCommonObject.Type.ToLowerInvariant() == "core/component")
        {
            cache.Set(key: "component|" + updatedCommonObject.Name.ToLower(), item: jsonBroker.ParseJson<Component>(json: updatedCommonObject.Json));

            CommonObject latestSetObject = cache.GetLatestSet()
                .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Type == "ContentManagement/Component");

            latestSetObject.Version = updatedCommonObject.Version;
            latestSetObject.Key = updatedCommonObject.Key;
            latestSetObject.Type = updatedCommonObject.Type;
            latestSetObject.Json = updatedCommonObject.Json;
            latestSetObject.Culture = updatedCommonObject.Culture;
            latestSetObject.Name = updatedCommonObject.Name;
            latestSetObject.Description = updatedCommonObject.Description;
            latestSetObject.LastUpdated = updatedCommonObject.LastUpdated;
            latestSetObject.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
            latestSetObject.CreatedBy = updatedCommonObject.CreatedBy;
        }
        else
        {
            if (updatedCommonObject.Type.ToLowerInvariant() == "core/resource")
            {
                cache.Set(key: $"resource|{updatedCommonObject.Key?.ToLower() ?? string.Empty}-{updatedCommonObject.Name?.ToLower() ?? string.Empty}-{updatedCommonObject.Culture?.ToLower() ?? string.Empty}", item: jsonBroker.ParseJson<Resource>(json: updatedCommonObject.Json));

                CommonObject latestSetObject2 = cache.GetLatestSet()
                    .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Key.ToLowerInvariant() == updatedCommonObject.Key.ToLowerInvariant() && r.Name == updatedCommonObject.Name.ToLowerInvariant() && r.Culture.ToLowerInvariant() == updatedCommonObject.Culture.ToLowerInvariant() && r.Type == "ContentManagement/Resource");

                latestSetObject2.Version = updatedCommonObject.Version;
                latestSetObject2.Key = updatedCommonObject.Key;
                latestSetObject2.Type = updatedCommonObject.Type;
                latestSetObject2.Json = updatedCommonObject.Json;
                latestSetObject2.Culture = updatedCommonObject.Culture;
                latestSetObject2.Name = updatedCommonObject.Name;
                latestSetObject2.Description = updatedCommonObject.Description;
                latestSetObject2.LastUpdated = updatedCommonObject.LastUpdated;
                latestSetObject2.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
                latestSetObject2.CreatedBy = updatedCommonObject.CreatedBy;
            }
            else
            {
                if (updatedCommonObject.Type.ToLowerInvariant() == "core/script")
                {
                    CommonObject latestSetObject3 = cache.GetLatestSet()
                        .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Type == "ContentManagement/Script");

                    latestSetObject3.Version = updatedCommonObject.Version;
                    latestSetObject3.Key = updatedCommonObject.Key;
                    latestSetObject3.Type = updatedCommonObject.Type;
                    latestSetObject3.Json = updatedCommonObject.Json;
                    latestSetObject3.Culture = updatedCommonObject.Culture;
                    latestSetObject3.Name = updatedCommonObject.Name;
                    latestSetObject3.Description = updatedCommonObject.Description;
                    latestSetObject3.LastUpdated = updatedCommonObject.LastUpdated;
                    latestSetObject3.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
                    latestSetObject3.CreatedBy = updatedCommonObject.CreatedBy;
                    cache.Set(key: "script|" + updatedCommonObject.Name.ToLower(), item: jsonBroker.ParseJson<Script>(json: updatedCommonObject.Json));
                }
            }
        }

        return updatedCommonObject;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int commonObjectId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [commonObjectId]);
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_delete");
        await service.DeleteAsync(commonObjectId: commonObjectId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<CommonObject>>> AddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject) =>
        TryCatch<IEnumerable<OperationResult<CommonObject>>>(operation: async () =>
    {
        ValidateOrUpdateCommonObjectResultOnAdd(inputs: [newCommonObject]);
        ValidateCommonObjects(commonObjects: newCommonObject, parameterName: "items");
        List<OperationResult<CommonObject>> results = new List<OperationResult<CommonObject>>();

        foreach (CommonObject item in newCommonObject)
        {
            try
            {
                CommonObject savedItem = item.Id < 1 ? await ExecuteAddCommonObjectAsync(newCommonObject: item) : await ExecuteUpdateCommonObjectAsync(updatedCommonObject: item);

                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllCommonObjectAsync(IEnumerable<CommonObject> deletedCommonObject) =>
        TryCatch(operation: async () =>
    {
        ValidateAllCommonObjectOnDelete(inputs: [deletedCommonObject]);
        ValidateCommonObjects(commonObjects: deletedCommonObject, parameterName: "items");

        foreach (CommonObject item in deletedCommonObject)
        {
            await ExecuteDeleteAsync(commonObjectId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateId(int commonObjectId, string parameterName) =>
        ThrowIf(condition: commonObjectId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateType(string type, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: type), message: parameterName + " is required.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(condition: commonObject == null, message: parameterName + " is required.");

    private static void ValidateCommonObjects(IEnumerable<CommonObject> commonObjects, string parameterName) =>
        ThrowIf(condition: commonObjects == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private async ValueTask<CommonObject> ExecuteAddCommonObjectAsync(CommonObject newCommonObject)
    {
        ValidateCommonObject(commonObject: newCommonObject, parameterName: "entity");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_create");
        return await service.AddCommonObjectAsync(newCommonObject: newCommonObject);
    }

    private async ValueTask<IEnumerable<OperationResult<CommonObject>>> ExecuteAddOrUpdateCommonObjectResult(IEnumerable<CommonObject> newCommonObject)
    {
        ValidateCommonObjects(commonObjects: newCommonObject, parameterName: "items");
        List<OperationResult<CommonObject>> results = new List<OperationResult<CommonObject>>();

        foreach (CommonObject item in newCommonObject)
        {
            try
            {
                CommonObject savedItem = item.Id < 1 ? await ExecuteAddCommonObjectAsync(newCommonObject: item) : await ExecuteUpdateCommonObjectAsync(updatedCommonObject: item);

                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<CommonObject>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private async ValueTask ExecuteDeleteAsync(int commonObjectId)
    {
        ValidateId(commonObjectId: commonObjectId, parameterName: "id");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_delete");
        await service.DeleteAsync(commonObjectId: commonObjectId);
    }

    private IEnumerable<CommonObject> ExecuteLatestCommonObject(string type)
    {
        ValidateType(type: type, parameterName: "type");

        return cache.GetLatestSet()
            .Where(predicate: item => item.Type == type);
    }

    private async ValueTask<CommonObject> ExecuteUpdateCommonObjectAsync(CommonObject updatedCommonObject)
    {
        ValidateCommonObject(commonObject: updatedCommonObject, parameterName: "entity");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_create");
        authorizationManager.Authorize(appId: null, privilege: "commonobject_update");

        int newVersionCount = service.GetAllCommonObject()
            .Count(predicate: (CommonObject c) => c.Name == updatedCommonObject.Name && c.Type == updatedCommonObject.Type && c.Culture == updatedCommonObject.Culture && c.Key == updatedCommonObject.Key) + 1;

        int newVersionFromField = service.GetAllCommonObject()
            .Where(predicate: item => item.Name == updatedCommonObject.Name && item.Type == updatedCommonObject.Type && item.Culture == updatedCommonObject.Culture && item.Key == updatedCommonObject.Key)
            .OrderByDescending(keySelector: item => item.Version)
            .FirstOrDefault()?.Version ?? 1;

        updatedCommonObject.Id = 0;
        updatedCommonObject.Version = ((newVersionCount > newVersionFromField) ? newVersionCount : (newVersionFromField + 1));
        updatedCommonObject.CreatedOn = DateTimeOffset.Now;
        updatedCommonObject.LastUpdated = DateTimeOffset.Now;
        updatedCommonObject.LastUpdatedBy = GetCurrentUser().Id;
        updatedCommonObject.CreatedBy = GetCurrentUser().Id;
        updatedCommonObject = await service.AddCommonObjectAsync(newCommonObject: updatedCommonObject);

        if (updatedCommonObject.Type.ToLowerInvariant() == "core/component")
        {
            cache.Set(key: "component|" + updatedCommonObject.Name.ToLower(), item: jsonBroker.ParseJson<Component>(json: updatedCommonObject.Json));

            CommonObject latestSetObject = cache.GetLatestSet()
                .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Type == "ContentManagement/Component");

            latestSetObject.Version = updatedCommonObject.Version;
            latestSetObject.Key = updatedCommonObject.Key;
            latestSetObject.Type = updatedCommonObject.Type;
            latestSetObject.Json = updatedCommonObject.Json;
            latestSetObject.Culture = updatedCommonObject.Culture;
            latestSetObject.Name = updatedCommonObject.Name;
            latestSetObject.Description = updatedCommonObject.Description;
            latestSetObject.LastUpdated = updatedCommonObject.LastUpdated;
            latestSetObject.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
            latestSetObject.CreatedBy = updatedCommonObject.CreatedBy;
        }
        else
        {
            if (updatedCommonObject.Type.ToLowerInvariant() == "core/resource")
            {
                cache.Set(key: $"resource|{updatedCommonObject.Key?.ToLower() ?? string.Empty}-{updatedCommonObject.Name?.ToLower() ?? string.Empty}-{updatedCommonObject.Culture?.ToLower() ?? string.Empty}", item: jsonBroker.ParseJson<Resource>(json: updatedCommonObject.Json));

                CommonObject latestSetObject2 = cache.GetLatestSet()
                    .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Key.ToLowerInvariant() == updatedCommonObject.Key.ToLowerInvariant() && r.Name == updatedCommonObject.Name.ToLowerInvariant() && r.Culture.ToLowerInvariant() == updatedCommonObject.Culture.ToLowerInvariant() && r.Type == "ContentManagement/Resource");

                latestSetObject2.Version = updatedCommonObject.Version;
                latestSetObject2.Key = updatedCommonObject.Key;
                latestSetObject2.Type = updatedCommonObject.Type;
                latestSetObject2.Json = updatedCommonObject.Json;
                latestSetObject2.Culture = updatedCommonObject.Culture;
                latestSetObject2.Name = updatedCommonObject.Name;
                latestSetObject2.Description = updatedCommonObject.Description;
                latestSetObject2.LastUpdated = updatedCommonObject.LastUpdated;
                latestSetObject2.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
                latestSetObject2.CreatedBy = updatedCommonObject.CreatedBy;
            }
            else
            {
                if (updatedCommonObject.Type.ToLowerInvariant() == "core/script")
                {
                    CommonObject latestSetObject3 = cache.GetLatestSet()
                        .First(predicate: (CommonObject r) => r.Name.ToLowerInvariant() == updatedCommonObject.Name.ToLowerInvariant() && r.Type == "ContentManagement/Script");

                    latestSetObject3.Version = updatedCommonObject.Version;
                    latestSetObject3.Key = updatedCommonObject.Key;
                    latestSetObject3.Type = updatedCommonObject.Type;
                    latestSetObject3.Json = updatedCommonObject.Json;
                    latestSetObject3.Culture = updatedCommonObject.Culture;
                    latestSetObject3.Name = updatedCommonObject.Name;
                    latestSetObject3.Description = updatedCommonObject.Description;
                    latestSetObject3.LastUpdated = updatedCommonObject.LastUpdated;
                    latestSetObject3.LastUpdatedBy = updatedCommonObject.LastUpdatedBy;
                    latestSetObject3.CreatedBy = updatedCommonObject.CreatedBy;
                    cache.Set(key: "script|" + updatedCommonObject.Name.ToLower(), item: jsonBroker.ParseJson<Script>(json: updatedCommonObject.Json));
                }
            }
        }

        return updatedCommonObject;
    }
}