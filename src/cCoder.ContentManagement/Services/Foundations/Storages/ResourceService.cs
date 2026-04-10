using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ResourceService(IResourceBroker resourceBroker, IAuthorizationBroker authorizationBroker) : IResourceService
{
    public Resource Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Resource i) => i.Id == id);
        }

        Resource resource = GetAll().FirstOrDefault((Resource i) => i.Id == id);
        if (resource != null)
        {
            return resource;
        }
        Resource resource2 = GetAll(ignoreFilters: true).FirstOrDefault((Resource i) => i.Id == id);
        if (resource2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Resource> GetAll(bool ignoreFilters = false)
    {
        return resourceBroker.GetAllResources(ignoreFilters);
    }

    public async ValueTask<Resource> AddAsync(Resource resource)
    {
        ValidateResource(resource, "resource");
        authorizationBroker.Authorize(resource.AppId, "Resource_create");
        Resource newResource = CreateStorageResource(resource);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newResource.CreatedOn = DateTimeOffset.UtcNow);
        newResource.CreatedBy = currentUserId;
        newResource.LastUpdated = now;
        newResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.AddResourceAsync(newResource);
        result.App = resource.App;
        return result;
    }

    public async ValueTask<Resource> UpdateAsync(Resource resource)
    {
        ValidateResource(resource, "resource");
        authorizationBroker.Authorize(resource.AppId, "Resource_update");
        Resource updateResource = CreateStorageResource(resource);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateResource.LastUpdated = now;
        updateResource.LastUpdatedBy = currentUserId;
        Resource result = await resourceBroker.UpdateResourceAsync(updateResource);
        result.App = resource.App;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Resource resource;
        try
        {
            resource = Get(id);
        }
        catch (SecurityException)
        {
            resource = Get(id, ignoreFilters: true);
        }

        if (resource == null)
        {
            return;
        }

        authorizationBroker.Authorize(resource.AppId, "Resource_delete");
        await resourceBroker.DeleteResourceAsync(CreateStorageResource(resource));
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
