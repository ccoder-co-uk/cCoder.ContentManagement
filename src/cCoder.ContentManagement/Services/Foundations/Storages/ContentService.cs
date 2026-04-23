using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ContentService(
    IContentBroker contentBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IContentService
{
    public Content Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Content i) => i.Id == id);
        }

        Content content = GetAll().FirstOrDefault((Content i) => i.Id == id);
        if (content != null)
        {
            return content;
        }
        Content content2 = GetAll(ignoreFilters: true).FirstOrDefault((Content i) => i.Id == id);
        if (content2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Content> GetAll(bool ignoreFilters = false)
    {
        return contentBroker.GetAllContents(ignoreFilters);
    }

    public async ValueTask<Content> AddAsync(Content content)
    {
        ValidateContent(content, "content");
        authorizationBroker.Authorize(GetAppId(content.PageId), "Content_create");
        Content result = await contentBroker.AddContentAsync(CreateStorageContent(content));
        content.Id = result.Id;
        content.PageId = result.PageId;
        content.CultureId = result.CultureId;
        content.Name = result.Name;
        content.Html = result.Html;
        return content;
    }

    public async ValueTask<Content> UpdateAsync(Content content)
    {
        ValidateContent(content, "content");
        authorizationBroker.Authorize(GetAppId(content.PageId), "Content_update");
        Content result = await contentBroker.UpdateContentAsync(CreateStorageContent(content));
        content.Id = result.Id;
        content.PageId = result.PageId;
        content.CultureId = result.CultureId;
        content.Name = result.Name;
        content.Html = result.Html;
        return content;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Content content;
        try
        {
            content = Get(id);
        }
        catch (SecurityException)
        {
            content = Get(id, ignoreFilters: true);
        }

        if (content == null)
        {
            return;
        }

        authorizationBroker.Authorize(GetAppId(content.PageId), "Content_delete");
        await contentBroker.DeleteContentAsync(CreateStorageContent(content));
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
            .Where(page => page.Id == pageId)
            .Select(page => (int?)page.AppId)
            .FirstOrDefault();
    }
}
