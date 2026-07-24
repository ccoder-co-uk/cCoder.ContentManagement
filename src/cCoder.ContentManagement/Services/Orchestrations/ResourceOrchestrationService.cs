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
    public Resource Get(int id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Resource> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Resource> AddAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");

        Resource result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseResourceAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Resource> UpdateAsync(Resource entity)
    {
        ValidateResource(resource: entity, parameterName: "entity");

        Resource result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseResourceUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Resource entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: resource => resource.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseResourceDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Resource[] resourcesToDelete = [.. GetAll(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == appId)];

        if (resourcesToDelete.Length > 0)
        {
            await DeleteAllAsync(items: resourcesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result<Resource>>> AddOrUpdate(IEnumerable<Resource> items)
    {
        Resource[] resources = ValidateResources(resources: items, parameterName: "items")
            .ToArray();

        List<Result<Resource>> results = new();

        foreach (Resource resource in resources)
        {
            try
            {
                Resource result = resource.Id <= 0
                    ? await AddAsync(entity: resource)
                    : await UpdateAsync(entity: resource);

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

        var dbVersions = processingService.GetAll()
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

        await AddOrUpdate(items: validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Resource> items)
    {
        Resource[] resources = ValidateResources(resources: items, parameterName: "items")
            .ToArray();

        foreach (Resource resource in resources)
        {
            await DeleteAsync(id: resource.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return id;
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