using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using Content = cCoder.Data.Models.CMS.Content;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Content>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ContentOrchestrationService(
    IContentProcessingService processingService,
    IContentEventProcessingService eventService) : IContentOrchestrationService
{
    public Content Get(int id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Content> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Content> AddAsync(Content entity)
    {
        ValidateContent(entity, "entity");

        Content result = await processingService.AddAsync(entity);
        await eventService.RaiseContentAddEventAsync(result);
        return result;
    }

    public async ValueTask<Content> UpdateAsync(Content entity)
    {
        ValidateContent(entity, "entity");

        Content result = await processingService.UpdateAsync(entity);
        await eventService.RaiseContentUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Content entity = processingService.Get(id);
        await eventService.RaiseContentDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Content> items)
    {
        Content[] contents = ValidateContents(items, "items").ToArray();
        List<Result> results = new();

        foreach (Content content in contents)
        {
            try
            {
                Content result = content.Id <= 0
                    ? await AddAsync(content)
                    : await UpdateAsync(content);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = content.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
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
        Content[] contents = ValidateContents(items, "items").ToArray();

        foreach (Content content in contents)
        {
            await DeleteAsync(content.Id);
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

    private static Content ValidateContent(Content content, string parameterName)
    {
        if (content == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return content;
    }

    private static IEnumerable<Content> ValidateContents(IEnumerable<Content> contents, string parameterName)
    {
        if (contents == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return contents;
    }
}
