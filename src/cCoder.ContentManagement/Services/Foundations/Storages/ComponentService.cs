// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ComponentService(IComponentBroker componentBroker, IAuthorizationManager authorizationManager) : IComponentService
{
    public Component GetComponent(int componentId, bool ignoreFilters = false) =>
        TryCatch<Component>(operation: () =>
    {
        ValidateComponentOnGet(inputs: [componentId, ignoreFilters]);
        ValidateId(componentId: componentId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllComponent(ignoreFilters: true)
                .FirstOrDefault(predicate: (Component i) => i.Id == componentId);
        }

        Component component = ExecuteGetAllComponent()
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component != null)
        {
            return component;
        }

        Component component2 = ExecuteGetAllComponent(ignoreFilters: true)
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Component> GetAllComponent(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Component>>(operation: () =>
    {
        ValidateAllComponentOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? componentBroker.GetAllComponentsIgnoringFilters()
            : componentBroker.GetAllComponents();
    });

    public ValueTask<Component> AddComponentAsync(Component newComponent) =>
        TryCatch<Component>(operation: async () =>
    {
        ValidateComponentOnAdd(inputs: [newComponent]);
        ValidateComponent(component: newComponent, parameterName: "component");
        authorizationManager.Authorize(appId: newComponent.AppId, privilege: "Component_create");
        Component storageComponent = CreateStorageComponent(newComponent: newComponent);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storageComponent.CreatedOn = DateTimeOffset.UtcNow);
        storageComponent.CreatedBy = currentUserId;
        storageComponent.LastUpdated = now;
        storageComponent.LastUpdatedBy = currentUserId;
        Component result = await componentBroker.AddComponentAsync(newComponent: storageComponent);
        newComponent.Id = result.Id;
        newComponent.Name = result.Name;
        newComponent.Description = result.Description;
        newComponent.LastUpdated = result.LastUpdated;
        newComponent.LastUpdatedBy = result.LastUpdatedBy;
        newComponent.CreatedOn = result.CreatedOn;
        newComponent.CreatedBy = result.CreatedBy;
        newComponent.AppId = result.AppId;
        newComponent.ResourceKey = result.ResourceKey;
        newComponent.Content = result.Content;
        newComponent.Script = result.Script;
        newComponent.Key = result.Key;
        return newComponent;

    }, isValueTask: true);

    public ValueTask<Component> UpdateComponentAsync(Component updatedComponent) =>
        TryCatch<Component>(operation: async () =>
    {
        ValidateComponentOnUpdate(inputs: [updatedComponent]);
        ValidateComponent(component: updatedComponent, parameterName: "component");
        authorizationManager.Authorize(appId: updatedComponent.AppId, privilege: "Component_update");
        Component updateComponent = CreateStorageComponent(newComponent: updatedComponent);

        string currentUserId = authorizationManager.GetCurrentUser()
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

    }, isValueTask: true);

    public ValueTask DeleteAsync(int componentId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [componentId]);
        ValidateId(componentId: componentId, parameterName: "id");
        Component component;

        try
        {
            component = ExecuteGetComponent(componentId: componentId);
        }
        catch (SecurityException)
        {
            component = ExecuteGetComponent(componentId: componentId, ignoreFilters: true);
        }

        if (component == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: component.AppId, privilege: "Component_delete");
        await componentBroker.DeleteComponentAsync(deletedComponent: CreateStorageComponent(newComponent: component));

    }, isValueTask: true);

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

    private IQueryable<Component> ExecuteGetAllComponent(bool ignoreFilters = false) =>
        (ignoreFilters
            ? componentBroker.GetAllComponentsIgnoringFilters()
            : componentBroker.GetAllComponents());

    private Component ExecuteGetComponent(int componentId, bool ignoreFilters = false)
    {
        ValidateId(componentId: componentId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllComponent(ignoreFilters: true)
                .FirstOrDefault(predicate: (Component i) => i.Id == componentId);
        }

        Component component = ExecuteGetAllComponent()
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component != null)
        {
            return component;
        }

        Component component2 = ExecuteGetAllComponent(ignoreFilters: true)
            .FirstOrDefault(predicate: (Component i) => i.Id == componentId);

        if (component2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}