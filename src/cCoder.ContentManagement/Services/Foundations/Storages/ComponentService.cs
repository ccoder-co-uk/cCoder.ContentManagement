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
    public Component GetComponent(int componentId, bool ignoreFilters = false)
    {
        ValidateId(componentId: componentId, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAllComponent(ignoreFilters: true)
                .FirstOrDefault(predicate: (Component i) => i.Id == componentId);
        }

        Component component = GetAllComponent()
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component != null)
        {
            return component;
        }

        Component component2 = GetAllComponent(ignoreFilters: true)
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Component> GetAllComponent(bool ignoreFilters = false) =>
        componentBroker.GetAllComponents(ignoreFilters: ignoreFilters);

    public async ValueTask<Component> AddComponentAsync(Component component)
    {
        ValidateComponent(component: component, parameterName: "component");
        authorizationBroker.Authorize(appId: component.AppId, privilege: "Component_create");
        Component newComponent = CreateStorageComponent(newComponent: component);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newComponent.CreatedOn = DateTimeOffset.UtcNow);
        newComponent.CreatedBy = currentUserId;
        newComponent.LastUpdated = now;
        newComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.AddComponentAsync(newComponent: newComponent);
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

    public async ValueTask<Component> UpdateComponentAsync(Component updatedComponent)
    {
        ValidateComponent(component: updatedComponent, parameterName: "component");
        authorizationBroker.Authorize(appId: updatedComponent.AppId, privilege: "Component_update");
        Component updateComponent = CreateStorageComponent(newComponent: updatedComponent);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateComponent.LastUpdated = now;
        updateComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.UpdateComponentAsync(updatedComponent: updateComponent);
        updatedComponent.Id = result.Id;
        updatedComponent.Name = result.Name;
        updatedComponent.Description = result.Description;
        updatedComponent.LastUpdated = result.LastUpdated;
        updatedComponent.LastUpdatedBy = result.LastUpdatedBy;
        updatedComponent.CreatedOn = result.CreatedOn;
        updatedComponent.CreatedBy = result.CreatedBy;
        updatedComponent.AppId = result.AppId;
        updatedComponent.ResourceKey = result.ResourceKey;
        updatedComponent.Content = result.Content;
        updatedComponent.Script = result.Script;
        updatedComponent.Key = result.Key;
        return updatedComponent;
    }

    public async ValueTask DeleteAsync(int componentId)
    {
        ValidateId(componentId: componentId, parameterName: "id");
        Component component;

        try
        {
            component = GetComponent(componentId: componentId);
        }
        catch (SecurityException)
        {
            component = GetComponent(componentId: componentId, ignoreFilters: true);
        }

        if (component == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: component.AppId, privilege: "Component_delete");
        await componentBroker.DeleteComponentAsync(deletedComponent: CreateStorageComponent(newComponent: component));
    }

    private static Component CreateStorageComponent(Component newComponent)
    {
        if (newComponent == null)
        {
            return null;
        }

        return new Component
        {
            Id = newComponent.Id,
            Name = newComponent.Name,
            Description = newComponent.Description,
            LastUpdated = newComponent.LastUpdated,
            LastUpdatedBy = newComponent.LastUpdatedBy,
            CreatedOn = newComponent.CreatedOn,
            CreatedBy = newComponent.CreatedBy,
            AppId = newComponent.AppId,
            ResourceKey = newComponent.ResourceKey,
            Content = newComponent.Content,
            Script = newComponent.Script,
            Key = newComponent.Key
        };
    }
}