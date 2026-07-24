// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ResourceService(IResourceBroker resourceBroker, IAuthorizationBroker authorizationBroker) : IResourceService
{
    public Resource GetResource(int resourceId, bool ignoreFilters = false) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateResourceOnGet(inputs: [resourceId, ignoreFilters]);
        ValidateId(resourceId: resourceId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllResource(ignoreFilters: true)
                .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);
        }

        Resource resource = ExecuteGetAllResource()
            .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);

        if (resource != null)
        {
            return resource;
        }

        Resource resource2 = ExecuteGetAllResource(ignoreFilters: true)
            .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);

        if (resource2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Resource>>(operation: () =>
    {
        ValidateAllResourceOnGet(inputs: [ignoreFilters]);
        return resourceBroker.GetAllResources(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Resource> AddResourceAsync(Resource newResource) =>
        TryCatch<Resource>(operation: async () =>
    {
        ValidateResourceOnAdd(inputs: [newResource]);
        ValidateResource(resource: newResource, parameterName: "resource");
        authorizationBroker.Authorize(appId: newResource.AppId, privilege: "Resource_create");
        Resource storageResource = CreateStorageResource(newResource: newResource);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageResource.CreatedOn = DateTimeOffset.UtcNow);
        storageResource.CreatedBy = currentUserId;
        storageResource.LastUpdated = now;
        storageResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.AddResourceAsync(newResource: storageResource);
        newResource.Id = result.Id;
        newResource.Name = result.Name;
        newResource.Description = result.Description;
        newResource.LastUpdated = result.LastUpdated;
        newResource.LastUpdatedBy = result.LastUpdatedBy;
        newResource.CreatedOn = result.CreatedOn;
        newResource.CreatedBy = result.CreatedBy;
        newResource.AppId = result.AppId;
        newResource.Key = result.Key;
        newResource.Culture = result.Culture;
        newResource.DisplayName = result.DisplayName;
        newResource.ShortDisplayName = result.ShortDisplayName;
        return newResource;

    }, isValueTask: true);

    public ValueTask<Resource> UpdateResourceAsync(Resource updatedResource) =>
        TryCatch<Resource>(operation: async () =>
    {
        ValidateResourceOnUpdate(inputs: [updatedResource]);
        ValidateResource(resource: updatedResource, parameterName: "resource");
        authorizationBroker.Authorize(appId: updatedResource.AppId, privilege: "Resource_update");
        Resource updateResource = CreateStorageResource(newResource: updatedResource);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateResource.LastUpdated = now;
        updateResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.UpdateResourceAsync(updatedResource: updateResource);
        updatedResource.Id = result.Id;
        updatedResource.Name = result.Name;
        updatedResource.Description = result.Description;
        updatedResource.LastUpdated = result.LastUpdated;
        updatedResource.LastUpdatedBy = result.LastUpdatedBy;
        updatedResource.CreatedOn = result.CreatedOn;
        updatedResource.CreatedBy = result.CreatedBy;
        updatedResource.AppId = result.AppId;
        updatedResource.Key = result.Key;
        updatedResource.Culture = result.Culture;
        updatedResource.DisplayName = result.DisplayName;
        updatedResource.ShortDisplayName = result.ShortDisplayName;
        return updatedResource;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int resourceId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [resourceId]);
        ValidateId(resourceId: resourceId, parameterName: "id");
        Resource resource;

        try
        {
            resource = ExecuteGetResource(resourceId: resourceId);
        }
        catch (SecurityException)
        {
            resource = ExecuteGetResource(resourceId: resourceId, ignoreFilters: true);
        }

        if (resource == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: resource.AppId, privilege: "Resource_delete");
        await resourceBroker.DeleteResourceAsync(deletedResource: CreateStorageResource(newResource: resource));

    }, isValueTask: true);

    private static Resource CreateStorageResource(Resource newResource)
    {
        if (newResource == null)
        {
            return null;
        }

        return new Resource
        {
            Id = newResource.Id,
            Name = newResource.Name,
            Description = newResource.Description,
            LastUpdated = newResource.LastUpdated,
            LastUpdatedBy = newResource.LastUpdatedBy,
            CreatedOn = newResource.CreatedOn,
            CreatedBy = newResource.CreatedBy,
            AppId = newResource.AppId,
            Key = newResource.Key,
            Culture = newResource.Culture,
            DisplayName = newResource.DisplayName,
            ShortDisplayName = newResource.ShortDisplayName
        };
    }

    private IQueryable<Resource> ExecuteGetAllResource(bool ignoreFilters = false) =>
        resourceBroker.GetAllResources(ignoreFilters: ignoreFilters);

    private Resource ExecuteGetResource(int resourceId, bool ignoreFilters = false)
    {
        ValidateId(resourceId: resourceId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllResource(ignoreFilters: true)
                .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);
        }

        Resource resource = ExecuteGetAllResource()
            .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);

        if (resource != null)
        {
            return resource;
        }

        Resource resource2 = ExecuteGetAllResource(ignoreFilters: true)
            .FirstOrDefault(predicate: (Resource i) => i.Id == resourceId);

        if (resource2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}