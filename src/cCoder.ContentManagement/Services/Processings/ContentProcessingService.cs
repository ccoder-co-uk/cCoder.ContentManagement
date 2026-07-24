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
    public Content GetContent(int contentId)
    {
        ValidateId(contentId: contentId, parameterName: "id");
        return service.GetContent(contentId: contentId);
    }

    public IQueryable<Content> GetAllContent(bool ignoreFilters = false) =>
        service.GetAllContent(ignoreFilters: ignoreFilters);

    public ValueTask<Content> AddContentAsync(Content newContent)
    {
        ValidateContent(content: newContent, parameterName: "entity");
        return service.AddContentAsync(newContent: newContent);
    }

    public ValueTask<Content> UpdateContentAsync(Content updatedContent)
    {
        ValidateContent(content: updatedContent, parameterName: "entity");
        return service.UpdateContentAsync(updatedContent: updatedContent);
    }

    public ValueTask DeleteAsync(int contentId)
    {
        ValidateId(contentId: contentId, parameterName: "id");
        return service.DeleteAsync(contentId: contentId);
    }

    public async ValueTask<IEnumerable<Result<Content>>> AddOrUpdateContentResult(IEnumerable<Content> newContent)
    {
        ValidateContents(contents: newContent, parameterName: "items");
        List<Result<Content>> results = new List<Result<Content>>();

        foreach (Content item in newContent)
        {
            try
            {
                Content savedItem = item.Id < 1 ? await ExecuteAddContentAsync(newContent: item) : await ExecuteUpdateContentAsync(updatedContent: item);

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

    public async ValueTask DeleteAllContentAsync(IEnumerable<Content> deletedContent)
    {
        ValidateContents(contents: deletedContent, parameterName: "items");

        foreach (Content item in deletedContent)
        {
            await ExecuteDeleteAsync(contentId: item.Id);
        }
    }

    private static void ValidateId(int contentId, string parameterName) =>
        ThrowIf(condition: contentId < 1, message: parameterName + " must be greater than 0.");

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

    private ValueTask<Content> ExecuteAddContentAsync(Content newContent)
    {
        ValidateContent(content: newContent, parameterName: "entity");
        return service.AddContentAsync(newContent: newContent);
    }

    private ValueTask ExecuteDeleteAsync(int contentId)
    {
        ValidateId(contentId: contentId, parameterName: "id");
        return service.DeleteAsync(contentId: contentId);
    }

    private ValueTask<Content> ExecuteUpdateContentAsync(Content updatedContent)
    {
        ValidateContent(content: updatedContent, parameterName: "entity");
        return service.UpdateContentAsync(updatedContent: updatedContent);
    }
}