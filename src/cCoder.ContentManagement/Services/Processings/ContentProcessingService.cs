// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ContentProcessingService(IContentService service) : IContentProcessingService
{
    public Content Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Content> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Content> AddAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");
        return service.AddAsync(content: entity);
    }

    public ValueTask<Content> UpdateAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");
        return service.UpdateAsync(content: entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Content>>> AddOrUpdate(IEnumerable<Content> items)
    {
        ValidateContents(contents: items, parameterName: "items");
        List<Result<Content>> results = new List<Result<Content>>();

        foreach (Content item in items)
        {
            try
            {
                Content savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Content>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Content>
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
        ValidateContents(contents: items, parameterName: "items");

        foreach (Content item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateContent(Content content, string parameterName) =>
        ThrowIf(condition: content == null, message: parameterName + " is required.");

    private static void ValidateContents(IEnumerable<Content> contents, string parameterName) =>
        ThrowIf(condition: contents == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}