// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceProcessingService(IResourceService service, IAuthorizationBroker authorizationBroker) : IResourceProcessingService
{
    private User User =>
        authorizationBroker.GetCurrentUser();

    public Resource Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Resource> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Resource> AddAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");
        entity.CreatedOn = DateTimeOffset.Now;
        entity.CreatedBy = User.Id;
        entity.LastUpdated = entity.CreatedOn;
        entity.LastUpdatedBy = User.Id;
        return service.AddAsync(resource: entity);
    }

    public ValueTask<Resource> UpdateAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");
        entity.LastUpdated = DateTimeOffset.Now;
        entity.LastUpdatedBy = User.Id;
        return service.UpdateAsync(resource: entity);
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        Resource resource = Get(id: id);

        if (resource == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(value: resource.Culture))
        {
            Resource[] allVersions = GetAll()
                .Where(predicate: item => item.AppId == resource.AppId && item.Key == resource.Key && item.Name == resource.Name)
                .ToArray();

            Resource[] array = allVersions;

            foreach (Resource version in array)
            {
                await service.DeleteAsync(id: version.Id);
            }
        }
        else
        {
            await service.DeleteAsync(id: id);
        }
    }

    public async ValueTask<IEnumerable<Result<Resource>>> AddOrUpdate(IEnumerable<Resource> items)
    {
        ValidateResources(resources: items, parameterName: "items");
        List<Result<Resource>> results = new List<Result<Resource>>();

        foreach (Resource item in items)
        {
            try
            {
                Resource savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Resource>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Resource>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Resource> items)
    {
        ValidateResources(resources: items, parameterName: "items");
        HashSet<string> deletedIds = new HashSet<string>();

        foreach (Resource item in items)
        {
            string itemId = item.Id.ToString();

            if (deletedIds.Contains(item: itemId))
            {
                continue;
            }

            if (string.IsNullOrEmpty(value: item.Culture))
            {
                Resource[] allVersions = GetAll()
                    .Where(predicate: resource => resource.AppId == item.AppId && resource.Key == item.Key && resource.Name == item.Name)
                    .ToArray();

                Resource[] array = allVersions;

                foreach (Resource version in array)
                {
                    deletedIds.Add(item: version.Id.ToString());
                }
            }
            else
            {
                deletedIds.Add(item: itemId);
            }

            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateResource(Resource resource, string parameterName) =>
        ThrowIf(condition: resource == null, message: parameterName + " is required.");

    private static void ValidateResources(IEnumerable<Resource> resources, string parameterName) =>
        ThrowIf(condition: resources == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}