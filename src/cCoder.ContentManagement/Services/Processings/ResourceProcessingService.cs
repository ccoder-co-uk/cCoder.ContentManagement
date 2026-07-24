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

internal partial class ResourceProcessingService(IResourceService service, IAuthorizationBroker authorizationBroker) : IResourceProcessingService
{
    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public Resource GetResource(int resourceId) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateResourceOnGet(inputs: [resourceId]);
        ValidateId(resourceId: resourceId, parameterName: "id");
        return service.GetResource(resourceId: resourceId);

    });

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Resource>>(operation: () =>
    {
        ValidateAllResourceOnGet(inputs: [ignoreFilters]);
        return service.GetAllResource(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Resource> AddResourceAsync(Resource newResource) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateResourceOnAdd(inputs: [newResource]);
        ValidateResource(resource: newResource, parameterName: "entity");
        newResource.CreatedOn = DateTimeOffset.Now;
        newResource.CreatedBy = GetCurrentUser().Id;
        newResource.LastUpdated = newResource.CreatedOn;
        newResource.LastUpdatedBy = GetCurrentUser().Id;
        return service.AddResourceAsync(newResource: newResource);

    }, isValueTask: true);

    public ValueTask<Resource> UpdateResourceAsync(Resource updatedResource) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateResourceOnUpdate(inputs: [updatedResource]);
        ValidateResource(resource: updatedResource, parameterName: "entity");
        updatedResource.LastUpdated = DateTimeOffset.Now;
        updatedResource.LastUpdatedBy = GetCurrentUser().Id;
        return service.UpdateResourceAsync(updatedResource: updatedResource);

    }, isValueTask: true);

    public ValueTask DeleteAsync(int resourceId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [resourceId]);
        ValidateId(resourceId: resourceId, parameterName: "id");
        Resource resource = ExecuteGetResource(resourceId: resourceId);

        if (resource == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(value: resource.Culture))
        {
            Resource[] allVersions = ExecuteGetAllResource()
                .Where(predicate: item => item.AppId == resource.AppId && item.Key == resource.Key && item.Name == resource.Name)
                .ToArray();

            Resource[] array = allVersions;

            foreach (Resource version in array)
            {
                await service.DeleteAsync(resourceId: version.Id);
            }
        }
        else
        {
            await service.DeleteAsync(resourceId: resourceId);
        }

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource) =>
        TryCatch<IEnumerable<Result<Resource>>>(operation: async () =>
    {
        ValidateOrUpdateResourceResultOnAdd(inputs: [newResource]);
        ValidateResources(resources: newResource, parameterName: "items");
        List<Result<Resource>> results = new List<Result<Resource>>();

        foreach (Resource item in newResource)
        {
            try
            {
                Resource savedItem = item.Id < 1 ? await ExecuteAddResourceAsync(newResource: item) : await ExecuteUpdateResourceAsync(updatedResource: item);

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

    }, isValueTask: true);

    public ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource) =>
        TryCatch(operation: async () =>
    {
        ValidateAllResourceOnDelete(inputs: [deletedResource]);
        ValidateResources(resources: deletedResource, parameterName: "items");
        HashSet<string> deletedIds = new HashSet<string>();

        foreach (Resource item in deletedResource)
        {
            string itemId = item.Id.ToString();

            if (deletedIds.Contains(item: itemId))
            {
                continue;
            }

            if (string.IsNullOrEmpty(value: item.Culture))
            {
                Resource[] allVersions = ExecuteGetAllResource()
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

            await ExecuteDeleteAsync(resourceId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateId(int resourceId, string parameterName) =>
        ThrowIf(condition: resourceId < 1, message: parameterName + " must be greater than 0.");

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

    private ValueTask<Resource> ExecuteAddResourceAsync(Resource newResource)
    {
        ValidateResource(resource: newResource, parameterName: "entity");
        newResource.CreatedOn = DateTimeOffset.Now;
        newResource.CreatedBy = GetCurrentUser().Id;
        newResource.LastUpdated = newResource.CreatedOn;
        newResource.LastUpdatedBy = GetCurrentUser().Id;
        return service.AddResourceAsync(newResource: newResource);
    }

    private async ValueTask ExecuteDeleteAsync(int resourceId)
    {
        ValidateId(resourceId: resourceId, parameterName: "id");
        Resource resource = ExecuteGetResource(resourceId: resourceId);

        if (resource == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(value: resource.Culture))
        {
            Resource[] allVersions = ExecuteGetAllResource()
                .Where(predicate: item => item.AppId == resource.AppId && item.Key == resource.Key && item.Name == resource.Name)
                .ToArray();

            Resource[] array = allVersions;

            foreach (Resource version in array)
            {
                await service.DeleteAsync(resourceId: version.Id);
            }
        }
        else
        {
            await service.DeleteAsync(resourceId: resourceId);
        }
    }

    private IQueryable<Resource> ExecuteGetAllResource(bool ignoreFilters = false) =>
        service.GetAllResource(ignoreFilters: ignoreFilters);

    private Resource ExecuteGetResource(int resourceId)
    {
        ValidateId(resourceId: resourceId, parameterName: "id");
        return service.GetResource(resourceId: resourceId);
    }

    private ValueTask<Resource> ExecuteUpdateResourceAsync(Resource updatedResource)
    {
        ValidateResource(resource: updatedResource, parameterName: "entity");
        updatedResource.LastUpdated = DateTimeOffset.Now;
        updatedResource.LastUpdatedBy = GetCurrentUser().Id;
        return service.UpdateResourceAsync(updatedResource: updatedResource);
    }
}