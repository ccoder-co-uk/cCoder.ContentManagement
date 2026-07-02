using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ResourceProcessingService(IResourceService service, IAuthorizationBroker authorizationBroker) : IResourceProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public Resource Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Resource> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

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
            return;

        if (string.IsNullOrEmpty(resource.Culture))
        {
            Resource[] allVersions = GetAll()
                .Where(item => item.AppId == resource.AppId && item.Key == resource.Key && item.Name == resource.Name)
                .ToArray();

            Resource[] array = allVersions;
            foreach (Resource version in array)
                await service.DeleteAsync(version.Id);
        }
        else
        {
            await service.DeleteAsync(id);
        }
    }

    public async ValueTask<IEnumerable<Result<Resource>>> AddOrUpdate(IEnumerable<Resource> items)
    {
        ValidateResources(items, "items");
        List<Result<Resource>> results = new List<Result<Resource>>();
        foreach (Resource item in items)
        {
            try
            {
                Resource savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Resource>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Resource>
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
                continue;

            if (string.IsNullOrEmpty(item.Culture))
            {
                Resource[] allVersions = GetAll()
                    .Where(resource => resource.AppId == item.AppId && resource.Key == item.Key && resource.Name == item.Name)
                    .ToArray();

                Resource[] array = allVersions;
                foreach (Resource version in array)
                    deletedIds.Add(version.Id.ToString());
            }
            else
            {
                deletedIds.Add(itemId);
            }
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateResource(Resource resource, string parameterName) =>
        ThrowIf(resource == null, parameterName + " is required.");

    private static void ValidateResources(IEnumerable<Resource> resources, string parameterName) =>
        ThrowIf(resources == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
