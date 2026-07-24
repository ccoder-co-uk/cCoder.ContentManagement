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
    public Content Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Content i) => i.Id == id);
        }

        Content content = GetAll()
            .FirstOrDefault(predicate: (Content i) => i.Id == id);

        if (content != null)
        {
            return content;
        }

        Content content2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Content i) => i.Id == id);

        if (content2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Content> GetAll(bool ignoreFilters = false) =>
        contentBroker.GetAllContents(ignoreFilters: ignoreFilters);

    public async ValueTask<Content> AddAsync(Content content)
    {
        ValidateContent(content: content, parameterName: "content");
        authorizationBroker.Authorize(appId: GetAppId(pageId: content.PageId), privilege: "Content_create");
        Content result = await contentBroker.AddContentAsync(entity: CreateStorageContent(content: content));
        content.Id = result.Id;
        content.PageId = result.PageId;
        content.CultureId = result.CultureId;
        content.Name = result.Name;
        content.Html = result.Html;
        return content;
    }

    public async ValueTask<Content> UpdateAsync(Content content)
    {
        ValidateContent(content: content, parameterName: "content");
        authorizationBroker.Authorize(appId: GetAppId(pageId: content.PageId), privilege: "Content_update");
        Content result = await contentBroker.UpdateContentAsync(entity: CreateStorageContent(content: content));
        content.Id = result.Id;
        content.PageId = result.PageId;
        content.CultureId = result.CultureId;
        content.Name = result.Name;
        content.Html = result.Html;
        return content;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        Content content;

        try
        {
            content = Get(id: id);
        }
        catch (SecurityException)
        {
            content = Get(id: id, ignoreFilters: true);
        }

        if (content == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: GetAppId(pageId: content.PageId), privilege: "Content_delete");
        await contentBroker.DeleteContentAsync(entity: CreateStorageContent(content: content));
    }

    private static Content CreateStorageContent(Content content)
    {
        if (content == null)
        {
            return null;
        }

        return new Content
        {
            Id = content.Id,
            PageId = content.PageId,
            CultureId = content.CultureId,
            Name = content.Name,
            Html = content.Html
        };
    }

    private int? GetAppId(int pageId)
    {
        return pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.Id == pageId)
            .Select(selector: page => (int?)page.AppId)
            .FirstOrDefault();
    }
}