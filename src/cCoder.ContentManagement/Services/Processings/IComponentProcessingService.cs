using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentProcessingService
{
    Component Get(int id);

    IQueryable<Component> GetAll(bool ignoreFilters = false);

    ValueTask<Component> AddAsync(Component entity);

    ValueTask<Component> UpdateAsync(Component entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Component>>> AddOrUpdate(IEnumerable<Component> items);

    ValueTask DeleteAllAsync(IEnumerable<Component> items);
}
