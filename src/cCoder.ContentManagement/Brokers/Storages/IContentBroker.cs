using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IContentBroker
{
    IQueryable<Content> GetAllContents(bool ignoreFilters);

    ValueTask<Content> AddContentAsync(Content entity);

    ValueTask<Content> UpdateContentAsync(Content entity);

    ValueTask<int> DeleteContentAsync(Content entity);

    ValueTask DeleteAllContentsAsync(IEnumerable<Content> items);
}
