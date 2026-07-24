// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ContentProcessingService(IContentService service) : IContentProcessingService
{
    public Content GetContent(int contentId) =>
        TryCatch<Content>(operation: () =>
    {
        ValidateContentOnGet(inputs: [contentId]);
        ValidateId(contentId: contentId, parameterName: "id");
        return service.GetContent(contentId: contentId);

    });

    public IQueryable<Content> GetAllContent(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Content>>(operation: () =>
    {
        ValidateAllContentOnGet(inputs: [ignoreFilters]);
        return service.GetAllContent(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Content> AddContentAsync(Content newContent) =>
        TryCatch<Content>(operation: () =>
    {
        ValidateContentOnAdd(inputs: [newContent]);
        ValidateContent(content: newContent, parameterName: "entity");
        return service.AddContentAsync(newContent: newContent);

    }, isValueTask: true);

    public ValueTask<Content> UpdateContentAsync(Content updatedContent) =>
        TryCatch<Content>(operation: () =>
    {
        ValidateContentOnUpdate(inputs: [updatedContent]);
        ValidateContent(content: updatedContent, parameterName: "entity");
        return service.UpdateContentAsync(updatedContent: updatedContent);

    }, isValueTask: true);

    public ValueTask DeleteAsync(int contentId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [contentId]);
        ValidateId(contentId: contentId, parameterName: "id");
        return service.DeleteAsync(contentId: contentId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Content>>> AddOrUpdateContentResult(IEnumerable<Content> newContent) =>
        TryCatch<IEnumerable<OperationResult<Content>>>(operation: async () =>
    {
        ValidateOrUpdateContentResultOnAdd(inputs: [newContent]);
        ValidateContents(contents: newContent, parameterName: "items");
        List<OperationResult<Content>> results = new List<OperationResult<Content>>();

        foreach (Content item in newContent)
        {
            try
            {
                Content savedItem = item.Id < 1 ? await ExecuteAddContentAsync(newContent: item) : await ExecuteUpdateContentAsync(updatedContent: item);

                results.Add(item: new OperationResult<Content>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Content>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllContentAsync(IEnumerable<Content> deletedContent) =>
        TryCatch(operation: async () =>
    {
        ValidateAllContentOnDelete(inputs: [deletedContent]);
        ValidateContents(contents: deletedContent, parameterName: "items");

        foreach (Content item in deletedContent)
        {
            await ExecuteDeleteAsync(contentId: item.Id);
        }

    }, isValueTask: true);

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