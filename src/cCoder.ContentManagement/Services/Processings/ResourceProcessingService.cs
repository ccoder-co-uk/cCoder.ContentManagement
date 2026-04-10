using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Resource = cCoder.Data.Models.CMS.Resource;
using User = cCoder.Data.Models.Security.User;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceProcessingService(IResourceService service, IAuthorizationBroker authorizationBroker) : IResourceProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public Resource Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Resource> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Resource> AddAsync(Resource entity)
    {
        ValidateResource(entity, "entity");
        entity.CreatedOn = DateTimeOffset.Now;
        entity.CreatedBy = User.Id;
        entity.LastUpdated = entity.CreatedOn;
        entity.LastUpdatedBy = User.Id;
        return service.AddAsync(entity);
    }

    public ValueTask<Resource> UpdateAsync(Resource entity)
    {
        ValidateResource(entity, "entity");
        entity.LastUpdated = DateTimeOffset.Now;
        entity.LastUpdatedBy = User.Id;
        return service.UpdateAsync(entity);
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Resource resource = Get(id);
        if (resource == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(resource.Culture))
        {
            Resource[] allVersions = (from r in GetAll()
                                      where r.AppId == resource.AppId && r.Key == resource.Key && r.Name == resource.Name
                                      select r).ToArray();
            Resource[] array = allVersions;
            foreach (Resource version in array)
            {
                await service.DeleteAsync(version.Id);
            }
        }
        else
        {
            await service.DeleteAsync(id);
        }
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Resource>>> AddOrUpdate(IEnumerable<Resource> items)
    {
        ValidateResources(items, "items");
        List<cCoder.ContentManagement.Models.Result<Resource>> results = new List<cCoder.ContentManagement.Models.Result<Resource>>();
        foreach (Resource item in items)
        {
            try
            {
                Resource savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<Resource>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<Resource>
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
        ValidateResources(items, "items");
        HashSet<string> deletedIds = new HashSet<string>();
        foreach (Resource item in items)
        {
            string itemId = item.Id.ToString();
            if (deletedIds.Contains(itemId))
            {
                continue;
            }
            if (string.IsNullOrEmpty(item.Culture))
            {
                Resource[] allVersions = (from r in GetAll()
                                          where r.AppId == item.AppId && r.Key == item.Key && r.Name == item.Name
                                          select r).ToArray();
                Resource[] array = allVersions;
                foreach (Resource version in array)
                {
                    deletedIds.Add(version.Id.ToString());
                }
            }
            else
            {
                deletedIds.Add(itemId);
            }
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateResource(Resource resource, string parameterName)
    {
        if (resource == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateResources(IEnumerable<Resource> resources, string parameterName)
    {
        if (resources == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
