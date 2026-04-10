using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Processings;
using Resource = cCoder.Data.Models.CMS.Resource;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Resource>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ResourceOrchestrationService(
    IResourceProcessingService processingService,
    IResourceEventProcessingService eventService) : IResourceOrchestrationService
{
    public Resource Get(int id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Resource> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Resource> AddAsync(Resource entity)
    {
        ValidateResource(entity, "entity");

        Resource result = await processingService.AddAsync(entity);
        await eventService.RaiseResourceAddEventAsync(result);
        return result;
    }

    public async ValueTask<Resource> UpdateAsync(Resource entity)
    {
        ValidateResource(entity, "entity");

        Resource result = await processingService.UpdateAsync(entity);
        await eventService.RaiseResourceUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Resource entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(resource => resource.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseResourceDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Resource[] resourcesToDelete = [.. GetAll(ignoreFilters: true).Where(resource => resource.AppId == appId)];

        if (resourcesToDelete.Length > 0)
        {
            await DeleteAllAsync(resourcesToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Resource> items)
    {
        Resource[] resources = ValidateResources(items, "items").ToArray();
        List<Result> results = new();

        foreach (Resource resource in resources)
        {
            try
            {
                Resource result = resource.Id <= 0
                    ? await AddAsync(resource)
                    : await UpdateAsync(resource);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = resource.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
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
        ValidateAppId(appId, "appId");

        Resource[] validatedItems = ValidateResources(items, "items").ToArray();

        var dbVersions = (from resource in processingService.GetAll()
                          where resource.AppId == appId
                          select new
                          {
                              resource.Id,
                              Match = $"{resource.Key}_{resource.Name}_{resource.Culture}"
                          }).ToArray();

        Array.ForEach(validatedItems, resource =>
        {
            resource.AppId = appId;
            resource.Id = dbVersions.FirstOrDefault(existing =>
                $"{resource.Key}_{resource.Name}_{resource.Culture}" == existing.Match)?.Id ?? 0;
        });

        await AddOrUpdate(validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Resource> items)
    {
        Resource[] resources = ValidateResources(items, "items").ToArray();

        foreach (Resource resource in resources)
        {
            await DeleteAsync(resource.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return id;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Resource ValidateResource(Resource resource, string parameterName)
    {
        if (resource == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return resource;
    }

    private static IEnumerable<Resource> ValidateResources(IEnumerable<Resource> resources, string parameterName)
    {
        if (resources == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return resources;
    }
}
