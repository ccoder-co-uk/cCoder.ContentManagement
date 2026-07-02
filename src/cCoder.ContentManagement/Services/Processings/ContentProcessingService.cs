using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ContentProcessingService(IContentService service) : IContentProcessingService
{
    public Content Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Content> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<Content> AddAsync(Content entity)
    {
        ValidateContent(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Content> UpdateAsync(Content entity)
    {
        ValidateContent(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<Content>>> AddOrUpdate(IEnumerable<Content> items)
    {
        ValidateContents(items, "items");
        List<Result<Content>> results = new List<Result<Content>>();
        foreach (Content item in items)
        {
            try
            {
                Content savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Content>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Content>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Content> items)
    {
        ValidateContents(items, "items");
        foreach (Content item in items)
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateContent(Content content, string parameterName) =>
        ThrowIf(content == null, parameterName + " is required.");

    private static void ValidateContents(IEnumerable<Content> contents, string parameterName) =>
        ThrowIf(contents == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
