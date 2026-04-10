using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IComponentService
{
    Component Get(int id, bool ignoreFilters = false);

    IQueryable<Component> GetAll(bool ignoreFilters = false);

    ValueTask<Component> AddAsync(Component component);

    ValueTask<Component> UpdateAsync(Component component);

    ValueTask DeleteAsync(int id);
}
