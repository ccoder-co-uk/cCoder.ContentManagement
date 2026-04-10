using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Services.Foundations.Storages;
using CommonObject = cCoder.Data.Models.CommonObject;
using Component = cCoder.Data.Models.CMS.Component;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

internal class CommonObjectProcessingService(ICommonObjectService service, ICommonObjectCache cache, IAuthorizationBroker authorizationBroker, IJsonBroker jsonBroker) : ICommonObjectProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public CommonObject Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<CommonObject> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public IEnumerable<CommonObject> Latest(string type)
    {
        ValidateType(type, "type");
        return cache.LatestSet
            .Where(item => item.Type == type);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<CommonObject>>> ImportAsync(IEnumerable<CommonObject> items)
    {
        ValidateCommonObjects(items, "items");
        CommonObject[] commonObjects = (items as CommonObject[]) ?? items.ToArray();
        IEnumerable<string> types = commonObjects.Select((CommonObject i) => i.Type).Distinct();
        List<cCoder.ContentManagement.Models.Result<CommonObject>> results = new List<cCoder.ContentManagement.Models.Result<CommonObject>>();
        List<CommonObject> adds = new List<CommonObject>();
        List<CommonObject> updates = new List<CommonObject>();
        foreach (string type in types)
        {
            IEnumerable<CommonObject> dbSet = Latest(type);
            CommonObject[] newSet = commonObjects.Where((CommonObject i) => i.Type == type).ToArray();
            CommonObject[] array = newSet;
            foreach (CommonObject entry in array)
            {
                CommonObject matchedDbEntry = dbSet.FirstOrDefault((CommonObject dbc) => MatchesOnCultureNameAndKey(dbc, entry));
                if (matchedDbEntry == null)
                {
                    entry.Id = 0;
                    entry.Version = 1;
                    adds.Add(entry);
                }
                else if (entry.CreatedOn > matchedDbEntry.CreatedOn || entry.LastUpdated > matchedDbEntry.LastUpdated)
                {
                    entry.Version = matchedDbEntry.Version + 1;
                    updates.Add(entry);
                }
            }
        }
        results.AddRange(await AddOrUpdate(adds));
        results.AddRange(await AddOrUpdate(updates));
        return results;
        static bool MatchesOnCultureNameAndKey(CommonObject dbc, CommonObject commonObject)
        {
            return dbc.Culture == commonObject.Culture && dbc.Name == commonObject.Name && dbc.Key == commonObject.Key;
        }
    }

    public async ValueTask<CommonObject> AddAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");
        authorizationBroker.Authorize(null, "commonobject_create");
        return await service.AddAsync(entity);
    }

    public async ValueTask<CommonObject> UpdateAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");
        authorizationBroker.Authorize(null, "commonobject_create");
        authorizationBroker.Authorize(null, "commonobject_update");
        int newVersionCount = service.GetAll().Count((CommonObject c) => c.Name == entity.Name && c.Type == entity.Type && c.Culture == entity.Culture && c.Key == entity.Key) + 1;
        int newVersionFromField = (from c in service.GetAll()
                                   where c.Name == entity.Name && c.Type == entity.Type && c.Culture == entity.Culture && c.Key == entity.Key
                                   orderby c.Version descending
                                   select c).FirstOrDefault()?.Version ?? 1;
        entity.Id = 0;
        entity.Version = ((newVersionCount > newVersionFromField) ? newVersionCount : (newVersionFromField + 1));
        entity.CreatedOn = DateTimeOffset.Now;
        entity.LastUpdated = DateTimeOffset.Now;
        entity.LastUpdatedBy = User.Id;
        entity.CreatedBy = User.Id;
        entity = await service.AddAsync(entity);
        if (entity.Type.ToLowerInvariant() == "core/component")
        {
            cache.Set("component|" + entity.Name.ToLower(), jsonBroker.ParseJson<Component>(entity.Json));
            cCoder.Data.Models.CommonObject latestSetObject = cache.LatestSet.First((cCoder.Data.Models.CommonObject r) => r.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant() && r.Type == "Core/Component");
            latestSetObject.Version = entity.Version;
            latestSetObject.Key = entity.Key;
            latestSetObject.Type = entity.Type;
            latestSetObject.Json = entity.Json;
            latestSetObject.Culture = entity.Culture;
            latestSetObject.Name = entity.Name;
            latestSetObject.Description = entity.Description;
            latestSetObject.LastUpdated = entity.LastUpdated;
            latestSetObject.LastUpdatedBy = entity.LastUpdatedBy;
            latestSetObject.CreatedBy = entity.CreatedBy;
        }
        else if (entity.Type.ToLowerInvariant() == "core/resource")
        {
            cache.Set($"resource|{entity.Key?.ToLower() ?? string.Empty}-{entity.Name?.ToLower() ?? string.Empty}-{entity.Culture?.ToLower() ?? string.Empty}", jsonBroker.ParseJson<Resource>(entity.Json));
            cCoder.Data.Models.CommonObject latestSetObject2 = cache.LatestSet.First((cCoder.Data.Models.CommonObject r) => r.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant() && r.Key.ToLowerInvariant() == entity.Key.ToLowerInvariant() && r.Name == entity.Name.ToLowerInvariant() && r.Culture.ToLowerInvariant() == entity.Culture.ToLowerInvariant() && r.Type == "Core/Resource");
            latestSetObject2.Version = entity.Version;
            latestSetObject2.Key = entity.Key;
            latestSetObject2.Type = entity.Type;
            latestSetObject2.Json = entity.Json;
            latestSetObject2.Culture = entity.Culture;
            latestSetObject2.Name = entity.Name;
            latestSetObject2.Description = entity.Description;
            latestSetObject2.LastUpdated = entity.LastUpdated;
            latestSetObject2.LastUpdatedBy = entity.LastUpdatedBy;
            latestSetObject2.CreatedBy = entity.CreatedBy;
        }
        else if (entity.Type.ToLowerInvariant() == "core/script")
        {
            cCoder.Data.Models.CommonObject latestSetObject3 = cache.LatestSet.First((cCoder.Data.Models.CommonObject r) => r.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant() && r.Type == "Core/Script");
            latestSetObject3.Version = entity.Version;
            latestSetObject3.Key = entity.Key;
            latestSetObject3.Type = entity.Type;
            latestSetObject3.Json = entity.Json;
            latestSetObject3.Culture = entity.Culture;
            latestSetObject3.Name = entity.Name;
            latestSetObject3.Description = entity.Description;
            latestSetObject3.LastUpdated = entity.LastUpdated;
            latestSetObject3.LastUpdatedBy = entity.LastUpdatedBy;
            latestSetObject3.CreatedBy = entity.CreatedBy;
            cache.Set("script|" + entity.Name.ToLower(), jsonBroker.ParseJson<Script>(entity.Json));
        }
        return entity;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        authorizationBroker.Authorize(null, "commonobject_delete");
        await service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<CommonObject>>> AddOrUpdate(IEnumerable<CommonObject> items)
    {
        ValidateCommonObjects(items, "items");
        List<cCoder.ContentManagement.Models.Result<CommonObject>> results = new List<cCoder.ContentManagement.Models.Result<CommonObject>>();
        foreach (CommonObject item in items)
        {
            try
            {
                CommonObject savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<CommonObject>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<CommonObject>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<CommonObject> items)
    {
        ValidateCommonObjects(items, "items");
        foreach (CommonObject item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateType(string type, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateCommonObjects(IEnumerable<CommonObject> commonObjects, string parameterName)
    {
        if (commonObjects == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}

