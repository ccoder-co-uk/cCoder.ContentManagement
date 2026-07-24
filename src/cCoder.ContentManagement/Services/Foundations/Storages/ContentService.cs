// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ContentService(
    IContentBroker contentBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IContentService
{
    public Content GetContent(int contentId, bool ignoreFilters = false)
    {
        ValidateId(contentId: contentId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllContent(ignoreFilters: true)
                .FirstOrDefault(predicate: (Content i) => i.Id == contentId);
        }

        Content content = ExecuteGetAllContent()
            .FirstOrDefault(predicate: (Content i) => i.Id == contentId);

        if (content != null)
        {
            return content;
        }

        Content content2 = ExecuteGetAllContent(ignoreFilters: true)
            .FirstOrDefault(predicate: (Content i) => i.Id == contentId);

        if (content2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Content> GetAllContent(bool ignoreFilters = false) =>
        contentBroker.GetAllContents(ignoreFilters: ignoreFilters);

    public async ValueTask<Content> AddContentAsync(Content newContent)
    {
        ValidateContent(content: newContent, parameterName: "content");
        authorizationBroker.Authorize(appId: GetAppId(pageId: newContent.PageId), privilege: "Content_create");
        Content result = await contentBroker.AddContentAsync(newContent: CreateStorageContent(newContent: newContent));
        newContent.Id = result.Id;
        newContent.PageId = result.PageId;
        newContent.CultureId = result.CultureId;
        newContent.Name = result.Name;
        newContent.Html = result.Html;
        return newContent;
    }

    public async ValueTask<Content> UpdateContentAsync(Content updatedContent)
    {
        ValidateContent(content: updatedContent, parameterName: "content");
        authorizationBroker.Authorize(appId: GetAppId(pageId: updatedContent.PageId), privilege: "Content_update");
        Content result = await contentBroker.UpdateContentAsync(updatedContent: CreateStorageContent(newContent: updatedContent));
        updatedContent.Id = result.Id;
        updatedContent.PageId = result.PageId;
        updatedContent.CultureId = result.CultureId;
        updatedContent.Name = result.Name;
        updatedContent.Html = result.Html;
        return updatedContent;
    }

    public async ValueTask DeleteAsync(int contentId)
    {
        ValidateId(contentId: contentId, parameterName: "id");
        Content content;

        try
        {
            content = ExecuteGetContent(contentId: contentId);
        }
        catch (SecurityException)
        {
            content = ExecuteGetContent(contentId: contentId, ignoreFilters: true);
        }

        if (content == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: GetAppId(pageId: content.PageId), privilege: "Content_delete");
        await contentBroker.DeleteContentAsync(deletedContent: CreateStorageContent(newContent: content));
    }

    private static Content CreateStorageContent(Content newContent)
    {
        if (newContent == null)
        {
            return null;
        }

        return new Content
        {
            Id = newContent.Id,
            PageId = newContent.PageId,
            CultureId = newContent.CultureId,
            Name = newContent.Name,
            Html = newContent.Html
        };
    }

    private int? GetAppId(int pageId)
    {
        return pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.Id == pageId)
            .Select(selector: page => (int?)page.AppId)
            .FirstOrDefault();
    }

    private IQueryable<Content> ExecuteGetAllContent(bool ignoreFilters = false) =>
        contentBroker.GetAllContents(ignoreFilters: ignoreFilters);

    private Content ExecuteGetContent(int contentId, bool ignoreFilters = false)
    {
        ValidateId(contentId: contentId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllContent(ignoreFilters: true)
                .FirstOrDefault(predicate: (Content i) => i.Id == contentId);
        }

        Content content = ExecuteGetAllContent()
            .FirstOrDefault(predicate: (Content i) => i.Id == contentId);

        if (content != null)
        {
            return content;
        }

        Content content2 = ExecuteGetAllContent(ignoreFilters: true)
            .FirstOrDefault(predicate: (Content i) => i.Id == contentId);

        if (content2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}