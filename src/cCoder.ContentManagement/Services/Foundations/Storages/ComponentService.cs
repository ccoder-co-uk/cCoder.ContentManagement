using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ComponentService(IComponentBroker componentBroker, IAuthorizationBroker authorizationBroker) : IComponentService
{
    public Component Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((Component i) => i.Id == id);
        }

        Component component = GetAll().FirstOrDefault((Component i) => i.Id == id);
        if (component != null)
        {
            return component;
        }
        Component component2 = GetAll(ignoreFilters: true).FirstOrDefault((Component i) => i.Id == id);
        if (component2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<Component> GetAll(bool ignoreFilters = false)
    {
        return componentBroker.GetAllComponents(ignoreFilters);
    }

    public async ValueTask<Component> AddAsync(Component component)
    {
        ValidateComponent(component, "component");
        authorizationBroker.Authorize(component.AppId, "Component_create");
        Component newComponent = CreateStorageComponent(component);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newComponent.CreatedOn = DateTimeOffset.UtcNow);
        newComponent.CreatedBy = currentUserId;
        newComponent.LastUpdated = now;
        newComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.AddComponentAsync(newComponent);
        component.Id = result.Id;
        component.Name = result.Name;
        component.Description = result.Description;
        component.LastUpdated = result.LastUpdated;
        component.LastUpdatedBy = result.LastUpdatedBy;
        component.CreatedOn = result.CreatedOn;
        component.CreatedBy = result.CreatedBy;
        component.AppId = result.AppId;
        component.ResourceKey = result.ResourceKey;
        component.Content = result.Content;
        component.Script = result.Script;
        component.Key = result.Key;
        return component;
    }

    public async ValueTask<Component> UpdateAsync(Component component)
    {
        ValidateComponent(component, "component");
        authorizationBroker.Authorize(component.AppId, "Component_update");
        Component updateComponent = CreateStorageComponent(component);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateComponent.LastUpdated = now;
        updateComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.UpdateComponentAsync(updateComponent);
        component.Id = result.Id;
        component.Name = result.Name;
        component.Description = result.Description;
        component.LastUpdated = result.LastUpdated;
        component.LastUpdatedBy = result.LastUpdatedBy;
        component.CreatedOn = result.CreatedOn;
        component.CreatedBy = result.CreatedBy;
        component.AppId = result.AppId;
        component.ResourceKey = result.ResourceKey;
        component.Content = result.Content;
        component.Script = result.Script;
        component.Key = result.Key;
        return component;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Component component;
        try
        {
            component = Get(id);
        }
        catch (SecurityException)
        {
            component = Get(id, ignoreFilters: true);
        }

        if (component == null)
        {
            return;
        }

        authorizationBroker.Authorize(component.AppId, "Component_delete");
        await componentBroker.DeleteComponentAsync(CreateStorageComponent(component));
    }

    private static Component CreateStorageComponent(Component component)
    {
        if (component == null)
        {
            return null;
        }

        return new Component
        {
            Id = component.Id,
            Name = component.Name,
            Description = component.Description,
            LastUpdated = component.LastUpdated,
            LastUpdatedBy = component.LastUpdatedBy,
            CreatedOn = component.CreatedOn,
            CreatedBy = component.CreatedBy,
            AppId = component.AppId,
            ResourceKey = component.ResourceKey,
            Content = component.Content,
            Script = component.Script,
            Key = component.Key
        };
    }
}
