using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ICultureBroker
{
    IQueryable<Culture> GetAllCultures(bool ignoreFilters);

    ValueTask<Culture> AddCultureAsync(Culture entity);

    ValueTask<Culture> UpdateCultureAsync(Culture entity);

    ValueTask<int> DeleteCultureAsync(Culture entity);

    ValueTask DeleteAllCulturesAsync(IEnumerable<Culture> items);
}
