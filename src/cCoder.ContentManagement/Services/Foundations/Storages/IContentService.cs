using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IContentService
{
    Content Get(int id, bool ignoreFilters = false);

    IQueryable<Content> GetAll(bool ignoreFilters = false);

    ValueTask<Content> AddAsync(Content content);

    ValueTask<Content> UpdateAsync(Content content);

    ValueTask DeleteAsync(int id);
}
