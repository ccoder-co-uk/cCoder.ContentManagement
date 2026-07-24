// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ResourceOrchestrationService(
    IResourceProcessingService processingService,
    IResourceEventProcessingService eventService) : IResourceOrchestrationService
{
    public Resource GetResource(int resourceId) =>
        processingService.GetResource(resourceId: ValidateId(resourceId: resourceId, parameterName: "id"));

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        processingService.GetAllResource(ignoreFilters: ignoreFilters);

    public async ValueTask<Resource> AddResourceAsync(Resource newResource)
    {
        ValidateResource(resource: newResource, parameterName: "entity");

        Resource result = await processingService.AddResourceAsync(newResource: newResource);
        await eventService.RaiseResourceAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Resource> UpdateResourceAsync(Resource updatedResource)
    {
        ValidateResource(resource: updatedResource, parameterName: "entity");

        Resource result = await processingService.UpdateResourceAsync(updatedResource: updatedResource);
        await eventService.RaiseResourceUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int resourceId)
    {
        ValidateId(resourceId: resourceId, parameterName: "id");

        Resource entity;

        try
        {
            entity = processingService.GetResource(resourceId: resourceId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllResource(ignoreFilters: true)
                .FirstOrDefault(predicate: resource => resource.Id == resourceId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseResourceDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(resourceId: resourceId);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Resource[] resourcesToDelete = [.. GetAllResource(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == appId)];

        if (resourcesToDelete.Length > 0)
        {
            await DeleteAllResourceAsync(deletedResource: resourcesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource)
    {
        Resource[] resources = ValidateResources(resources: newResource, parameterName: "items")
            .ToArray();

        List<Result<Resource>> results = new();

        foreach (Resource resource in resources)
        {
            try
            {
                Resource result = resource.Id <= 0
                    ? await AddResourceAsync(newResource: resource)
                    : await UpdateResourceAsync(updatedResource: resource);

                results.Add(item: new Result<Resource>
                {
                    Success = true,
                    Item = result,
                    Message = resource.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Resource>
                {
                    Success = false,
                    Item = resource,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportResourcesAsync(int appId, Resource[] items)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Resource[] validatedItems = ValidateResources(resources: items, parameterName: "items")
            .ToArray();

        var dbVersions = processingService.GetAllResource()
            .Where(predicate: resource => resource.AppId == appId)
            .Select(selector: resource => new
            {
                resource.Id,
                Match = $"{resource.Key}_{resource.Name}_{resource.Culture}"
            })
            .ToArray();

        Array.ForEach(array: validatedItems, action: resource =>
        {
            resource.AppId = appId;

            resource.Id = dbVersions.FirstOrDefault(predicate: existing =>
                $"{resource.Key}_{resource.Name}_{resource.Culture}" == existing.Match)?.Id ?? 0;
        });

        await AddOrUpdateResourceResult(newResource: validatedItems);
    }

    public async ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource)
    {
        Resource[] resources = ValidateResources(resources: deletedResource, parameterName: "items")
            .ToArray();

        foreach (Resource resource in resources)
        {
            await DeleteAsync(resourceId: resource.Id);
        }
    }

    private static int ValidateId(int resourceId, string parameterName)
    {
        if (resourceId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return resourceId;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Resource ValidateResource(Resource resource, string parameterName)
    {
        if (resource == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return resource;
    }

    private static IEnumerable<Resource> ValidateResources(IEnumerable<Resource> resources, string parameterName)
    {
        if (resources == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return resources;
    }
}