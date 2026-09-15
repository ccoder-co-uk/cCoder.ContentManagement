// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class ContentOrchestrationService(
    IContentProcessingService processingService,
    IContentEventProcessingService eventService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IContentOrchestrationService
{
    public Content GetContent(int contentId) =>
        TryCatch<Content>(operation: () =>
    {
        ValidateContentOnGet(inputs: [contentId]);
        return processingService.GetContent(contentId: ValidateId(contentId: contentId, parameterName: "id"));
    });

    public IQueryable<Content> GetAllContent(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Content>>(operation: () =>
    {
        ValidateAllContentOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllContent(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Content> AddContentAsync(Content newContent) =>
        TryCatch<Content>(operation: async () =>
    {
        ValidateContentOnAdd(inputs: [newContent]);
        ValidateContent(content: newContent, parameterName: "entity");
        Authorize(content: newContent, privilege: "Content_create");

        Content result = await processingService.AddContentAsync(newContent: newContent);

        await eventService.RaiseContentAddEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask<Content> UpdateContentAsync(Content updatedContent) =>
        TryCatch<Content>(operation: async () =>
    {
        ValidateContentOnUpdate(inputs: [updatedContent]);
        ValidateContent(content: updatedContent, parameterName: "entity");
        Authorize(content: updatedContent, privilege: "Content_update");

        Content result = await processingService.UpdateContentAsync(updatedContent: updatedContent);

        await eventService.RaiseContentUpdateEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int contentId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [contentId]);
        ValidateId(contentId: contentId, parameterName: "id");

        Content entity;

        try
        {
            entity = processingService.GetContent(contentId: contentId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllContent(ignoreFilters: true)
                .FirstOrDefault(predicate: content => content.Id == contentId);
        }

        if (entity == null)
        {
            return;
        }

        Authorize(content: entity, privilege: "Content_delete");

        await eventService.RaiseContentDeleteEventAsync(
            entity: entity,
            userId: authorizationProcessingService.GetCurrentUserId());

        await processingService.DeleteAsync(contentId: contentId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Content>>> AddOrUpdateContentResult(IEnumerable<Content> newContent) =>
        TryCatch<IEnumerable<OperationResult<Content>>>(operation: async () =>
    {
        ValidateOrUpdateContentResultOnAdd(inputs: [newContent]);

        Content[] contents = ValidateContents(contents: newContent, parameterName: "items")
            .ToArray();

        List<OperationResult<Content>> results = new();

        foreach (Content content in contents)
        {
            try
            {
                Content result = content.Id <= 0
                    ? await ExecuteAddContentAsync(newContent: content)
                    : await ExecuteUpdateContentAsync(updatedContent: content);

                results.Add(item: new OperationResult<Content>
                {
                    Success = true,
                    Item = result,
                    Message = content.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Content>
                {
                    Success = false,
                    Item = content,
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

        Content[] contents = ValidateContents(contents: deletedContent, parameterName: "items")
            .ToArray();

        foreach (Content content in contents)
        {
            await ExecuteDeleteAsync(contentId: content.Id);
        }

    }, isValueTask: true);

    private static int ValidateId(int contentId, string parameterName)
    {
        if (contentId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return contentId;
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

    private async ValueTask<Content> ExecuteAddContentAsync(Content newContent)
    {
        ValidateContent(content: newContent, parameterName: "entity");
        Authorize(content: newContent, privilege: "Content_create");

        Content result = await processingService.AddContentAsync(newContent: newContent);

        await eventService.RaiseContentAddEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;
    }

    private async ValueTask ExecuteDeleteAsync(int contentId)
    {
        ValidateId(contentId: contentId, parameterName: "id");

        Content entity;

        try
        {
            entity = processingService.GetContent(contentId: contentId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllContent(ignoreFilters: true)
                .FirstOrDefault(predicate: content => content.Id == contentId);
        }

        if (entity == null)
        {
            return;
        }

        Authorize(content: entity, privilege: "Content_delete");

        await eventService.RaiseContentDeleteEventAsync(
            entity: entity,
            userId: authorizationProcessingService.GetCurrentUserId());

        await processingService.DeleteAsync(contentId: contentId);
    }

    private async ValueTask<Content> ExecuteUpdateContentAsync(Content updatedContent)
    {
        ValidateContent(content: updatedContent, parameterName: "entity");
        Authorize(content: updatedContent, privilege: "Content_update");

        Content result = await processingService.UpdateContentAsync(updatedContent: updatedContent);

        await eventService.RaiseContentUpdateEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;
    }

    private void Authorize(Content content, string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = ResolveAppId(content: content),
                    Privilege = privilege
                }
            });

    private int? ResolveAppId(Content content) =>
        content.Page?.AppId
        ?? processingService.GetAppIdByPageId(pageId: content.PageId);
}