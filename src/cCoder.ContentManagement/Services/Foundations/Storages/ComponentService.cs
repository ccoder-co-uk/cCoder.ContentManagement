// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ComponentService(IComponentBroker componentBroker, IAuthorizationBroker authorizationBroker) : IComponentService
{
    public Component Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (Component i) => i.Id == id);
        }

        Component component = GetAll()
            .FirstOrDefault(predicate: (Component i) => i.Id == id);

        if (component != null)
        {
            return component;
        }

        Component component2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Component i) => i.Id == id);

        if (component2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Component> GetAll(bool ignoreFilters = false) =>
        componentBroker.GetAllComponents(ignoreFilters: ignoreFilters);

    public async ValueTask<Component> AddAsync(Component component)
    {
        ValidateComponent(component: component, parameterName: "component");
        authorizationBroker.Authorize(appId: component.AppId, privilege: "Component_create");
        Component newComponent = CreateStorageComponent(component: component);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newComponent.CreatedOn = DateTimeOffset.UtcNow);
        newComponent.CreatedBy = currentUserId;
        newComponent.LastUpdated = now;
        newComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.AddComponentAsync(entity: newComponent);
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
        ValidateComponent(component: component, parameterName: "component");
        authorizationBroker.Authorize(appId: component.AppId, privilege: "Component_update");
        Component updateComponent = CreateStorageComponent(component: component);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateComponent.LastUpdated = now;
        updateComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.UpdateComponentAsync(entity: updateComponent);
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
        ValidateId(id: id, parameterName: "id");
        Component component;

        try
        {
            component = Get(id: id);
        }
        catch (SecurityException)
        {
            component = Get(id: id, ignoreFilters: true);
        }

        if (component == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: component.AppId, privilege: "Component_delete");
        await componentBroker.DeleteComponentAsync(entity: CreateStorageComponent(component: component));
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