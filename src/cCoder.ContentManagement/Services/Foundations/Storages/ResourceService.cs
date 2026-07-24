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
    public Resource Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Resource i) => i.Id == id);
        }

        Resource resource = GetAll()
            .FirstOrDefault(predicate: (Resource i) => i.Id == id);

        if (resource != null)
        {
            return resource;
        }

        Resource resource2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Resource i) => i.Id == id);

        if (resource2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Resource> GetAll(bool ignoreFilters = false) =>
        resourceBroker.GetAllResources(ignoreFilters: ignoreFilters);

    public async ValueTask<Resource> AddAsync(Resource resource)
    {
        ValidateResource(resource: resource, parameterName: "resource");
        authorizationBroker.Authorize(appId: resource.AppId, privilege: "Resource_create");
        Resource newResource = CreateStorageResource(resource: resource);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newResource.CreatedOn = DateTimeOffset.UtcNow);
        newResource.CreatedBy = currentUserId;
        newResource.LastUpdated = now;
        newResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.AddResourceAsync(entity: newResource);
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

    public async ValueTask<Resource> UpdateAsync(Resource resource)
    {
        ValidateResource(resource: resource, parameterName: "resource");
        authorizationBroker.Authorize(appId: resource.AppId, privilege: "Resource_update");
        Resource updateResource = CreateStorageResource(resource: resource);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateResource.LastUpdated = now;
        updateResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.UpdateResourceAsync(entity: updateResource);
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

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        Resource resource;

        try
        {
            resource = Get(id: id);
        }
        catch (SecurityException)
        {
            resource = Get(id: id, ignoreFilters: true);
        }

        if (resource == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: resource.AppId, privilege: "Resource_delete");
        await resourceBroker.DeleteResourceAsync(entity: CreateStorageResource(resource: resource));
    }

    private static Resource CreateStorageResource(Resource resource)
    {
        if (resource == null)
        {
            return null;
        }

        return new Resource
        {
            Id = resource.Id,
            Name = resource.Name,
            Description = resource.Description,
            LastUpdated = resource.LastUpdated,
            LastUpdatedBy = resource.LastUpdatedBy,
            CreatedOn = resource.CreatedOn,
            CreatedBy = resource.CreatedBy,
            AppId = resource.AppId,
            Key = resource.Key,
            Culture = resource.Culture,
            DisplayName = resource.DisplayName,
            ShortDisplayName = resource.ShortDisplayName
        };
    }
}