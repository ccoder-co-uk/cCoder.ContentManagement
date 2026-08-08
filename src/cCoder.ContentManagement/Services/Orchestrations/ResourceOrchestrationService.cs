// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class ResourceOrchestrationService(
    IResourceProcessingService processingService,
    IResourceEventProcessingService eventService) : IResourceOrchestrationService
{
    public Resource GetResource(int resourceId) =>
        TryCatch<Resource>(operation: () =>
    {
        ValidateResourceOnGet(inputs: [resourceId]);
        return processingService.GetResource(resourceId: ValidateId(resourceId: resourceId, parameterName: "id"));
    });

    public IQueryable<Resource> GetAllResource(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Resource>>(operation: () =>
    {
        ValidateAllResourceOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllResource(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Resource> AddResourceAsync(Resource newResource) =>
        TryCatch<Resource>(operation: async () =>
    {
        ValidateResourceOnAdd(inputs: [newResource]);
        ValidateResource(resource: newResource, parameterName: "entity");

        Resource result = await processingService.AddResourceAsync(newResource: newResource);
        await eventService.RaiseResourceAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Resource> UpdateResourceAsync(Resource updatedResource) =>
        TryCatch<Resource>(operation: async () =>
    {
        ValidateResourceOnUpdate(inputs: [updatedResource]);
        ValidateResource(resource: updatedResource, parameterName: "entity");

        Resource result = await processingService.UpdateResourceAsync(updatedResource: updatedResource);
        await eventService.RaiseResourceUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int resourceId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [resourceId]);
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

    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateByAppIdOnDelete(inputs: [appId]);
        ValidateAppId(appId: appId, parameterName: "appId");

        Resource[] resourcesToDelete = [.. ExecuteGetAllResource(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == appId)];

        if (resourcesToDelete.Length > 0)
        {
            await ExecuteDeleteAllResourceAsync(deletedResource: resourcesToDelete);
        }

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Resource>>> AddOrUpdateResourceResult(IEnumerable<Resource> newResource) =>
        TryCatch<IEnumerable<OperationResult<Resource>>>(operation: async () =>
    {
        ValidateOrUpdateResourceResultOnAdd(inputs: [newResource]);

        Resource[] resources = ValidateResources(resources: newResource, parameterName: "items")
            .ToArray();

        List<OperationResult<Resource>> results = new();

        foreach (Resource resource in resources)
        {
            try
            {
                Resource result = resource.Id <= 0
                    ? await ExecuteAddResourceAsync(newResource: resource)
                    : await ExecuteUpdateResourceAsync(updatedResource: resource);

                results.Add(item: new OperationResult<Resource>
                {
                    Success = true,
                    Item = result,
                    Message = resource.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Resource>
                {
                    Success = false,
                    Item = resource,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask ImportResourcesAsync(int appId, Resource[] items) =>
        TryCatch(operation: async () =>
    {
        ValidateImportResourcesAsync(inputs: [appId, items]);
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

        await processingService.AddOrUpdateResourceResult(
            newResource: validatedItems);

    }, isValueTask: true);

    public ValueTask DeleteAllResourceAsync(IEnumerable<Resource> deletedResource) =>
        TryCatch(operation: async () =>
    {
        ValidateAllResourceOnDelete(inputs: [deletedResource]);

        Resource[] resources = ValidateResources(resources: deletedResource, parameterName: "items")
            .ToArray();

        foreach (Resource resource in resources)
        {
            await ExecuteDeleteAsync(resourceId: resource.Id);
        }

    }, isValueTask: true);

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

    private async ValueTask<IEnumerable<OperationResult<Resource>>> ExecuteAddOrUpdateResourceResult(IEnumerable<Resource> newResource)
    {
        Resource[] resources = ValidateResources(resources: newResource, parameterName: "items")
            .ToArray();

        List<OperationResult<Resource>> results = new();

        foreach (Resource resource in resources)
        {
            try
            {
                Resource result = resource.Id <= 0
                    ? await ExecuteAddResourceAsync(newResource: resource)
                    : await ExecuteUpdateResourceAsync(updatedResource: resource);

                results.Add(item: new OperationResult<Resource>
                {
                    Success = true,
                    Item = result,
                    Message = resource.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Resource>
                {
                    Success = false,
                    Item = resource,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private async ValueTask<Resource> ExecuteAddResourceAsync(Resource newResource)
    {
        ValidateResource(resource: newResource, parameterName: "entity");

        Resource result = await processingService.AddResourceAsync(newResource: newResource);
        await eventService.RaiseResourceAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask ExecuteDeleteAllResourceAsync(IEnumerable<Resource> deletedResource)
    {
        Resource[] resources = ValidateResources(resources: deletedResource, parameterName: "items")
            .ToArray();

        foreach (Resource resource in resources)
        {
            await ExecuteDeleteAsync(resourceId: resource.Id);
        }
    }

    private async ValueTask ExecuteDeleteAsync(int resourceId)
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

    private IQueryable<Resource> ExecuteGetAllResource(bool ignoreFilters = false) =>
        processingService.GetAllResource(ignoreFilters: ignoreFilters);

    private async ValueTask<Resource> ExecuteUpdateResourceAsync(Resource updatedResource)
    {
        ValidateResource(resource: updatedResource, parameterName: "entity");

        Resource result = await processingService.UpdateResourceAsync(updatedResource: updatedResource);
        await eventService.RaiseResourceUpdateEventAsync(entity: result);
        return result;
    }
}