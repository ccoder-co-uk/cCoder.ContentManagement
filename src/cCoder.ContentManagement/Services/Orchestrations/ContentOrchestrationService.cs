// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ContentOrchestrationService(
    IContentProcessingService processingService,
    IContentEventProcessingService eventService) : IContentOrchestrationService
{
    public Content Get(int id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Content> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Content> AddAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");

        Content result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseContentAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Content> UpdateAsync(Content entity)
    {
        ValidateContent(content: entity, parameterName: "entity");

        Content result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseContentUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Content entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: content => content.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseContentDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Content>>> AddOrUpdate(IEnumerable<Content> items)
    {
        Content[] contents = ValidateContents(contents: items, parameterName: "items")
            .ToArray();

        List<Result<Content>> results = new();

        foreach (Content content in contents)
        {
            try
            {
                Content result = content.Id <= 0
                    ? await AddAsync(entity: content)
                    : await UpdateAsync(entity: content);

                results.Add(item: new Result<Content>
                {
                    Success = true,
                    Item = result,
                    Message = content.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Content>
                {
                    Success = false,
                    Item = content,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Content> items)
    {
        Content[] contents = ValidateContents(contents: items, parameterName: "items")
            .ToArray();

        foreach (Content content in contents)
        {
            await DeleteAsync(id: content.Id);
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

    private static Content ValidateContent(Content content, string parameterName)
    {
        if (content == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return content;
    }

    private static IEnumerable<Content> ValidateContents(IEnumerable<Content> contents, string parameterName)
    {
        if (contents == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return contents;
    }
}