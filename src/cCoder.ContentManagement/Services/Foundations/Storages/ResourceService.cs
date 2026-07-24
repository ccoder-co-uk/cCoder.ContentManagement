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
    public Resource GetResource(int resourceId, bool ignoreFilters = false)
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

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        resourceBroker.GetAllResources(ignoreFilters: ignoreFilters);

    public async ValueTask<Resource> AddResourceAsync(Resource resource)
    {
        ValidateResource(resource: resource, parameterName: "resource");
        authorizationBroker.Authorize(appId: resource.AppId, privilege: "Resource_create");
        Resource newResource = CreateStorageResource(newResource: resource);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newResource.CreatedOn = DateTimeOffset.UtcNow);
        newResource.CreatedBy = currentUserId;
        newResource.LastUpdated = now;
        newResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.AddResourceAsync(newResource: newResource);
        resource.Id = result.Id;
        resource.Name = result.Name;
        resource.Description = result.Description;
        resource.LastUpdated = result.LastUpdated;
        resource.LastUpdatedBy = result.LastUpdatedBy;
        resource.CreatedOn = result.CreatedOn;
        resource.CreatedBy = result.CreatedBy;
        resource.AppId = result.AppId;
        resource.Key = result.Key;
        resource.Culture = result.Culture;
        resource.DisplayName = result.DisplayName;
        resource.ShortDisplayName = result.ShortDisplayName;
        return resource;
    }

    public async ValueTask<Resource> UpdateResourceAsync(Resource updatedResource)
    {
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
    }

    public async ValueTask DeleteAsync(int resourceId)
    {
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
    }

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