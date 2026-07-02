using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class ComponentReaderBroker(ICoreContextFactory coreContextFactory) : IComponentReaderBroker
{
    public IEnumerable<Component> GetComponents(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Components
            .IgnoreQueryFilters()
            .Where(component => component.AppId == appId)
            .AsNoTracking()
            .ToArray();
    }

    public Component GetComponent(int appId, string name)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return coreDataContext.Components
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(component =>
                component.AppId == appId
                && component.Name != null
                && component.Name.ToLower() == lowerName);
    }
}
