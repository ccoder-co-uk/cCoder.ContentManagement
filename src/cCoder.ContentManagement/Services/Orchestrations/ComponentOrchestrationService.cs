// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ComponentOrchestrationService(
    IComponentProcessingService processingService,
    IComponentEventProcessingService eventService) : IComponentOrchestrationService
{
    public Component GetComponent(int componentId)
    {
        ValidateId(componentId: componentId, parameterName: "id");
        return processingService.GetComponent(componentId: componentId);
    }

    public IQueryable<Component> GetAllComponent(bool ignoreFilters = false) =>
        processingService.GetAllComponent(ignoreFilters: ignoreFilters);

    public async ValueTask<Component> AddComponentAsync(Component newComponent)
    {
        ValidateComponent(component: newComponent, parameterName: "entity");

        Component result = await processingService.AddComponentAsync(newComponent: newComponent);
        await eventService.RaiseComponentAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Component> UpdateComponentAsync(Component updatedComponent)
    {
        ValidateComponent(component: updatedComponent, parameterName: "entity");

        Component result = await processingService.UpdateComponentAsync(updatedComponent: updatedComponent);
        await eventService.RaiseComponentUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int componentId)
    {
        ValidateId(componentId: componentId, parameterName: "id");

        Component entity;

        try
        {
            entity = processingService.GetComponent(componentId: componentId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllComponent(ignoreFilters: true)
                .FirstOrDefault(predicate: component => component.Id == componentId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseComponentDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(componentId: componentId);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Component[] componentsToDelete = [.. GetAllComponent(ignoreFilters: true)
            .Where(predicate: component => component.AppId == appId)];

        foreach (Component component in componentsToDelete)
        {
            await DeleteAsync(componentId: component.Id);
        }
    }

    public async ValueTask<IEnumerable<Result<Component>>> AddOrUpdateComponentResult(IEnumerable<Component> newComponent)
    {
        Component[] components = ValidateComponents(components: newComponent, parameterName: "items")
            .ToArray();

        List<Result<Component>> results = new();

        foreach (Component component in components)
        {
            try
            {
                Component result = component.Id <= 0
                    ? await AddComponentAsync(newComponent: component)
                    : await UpdateComponentAsync(updatedComponent: component);

                results.Add(item: new Result<Component>
                {
                    Success = true,
                    Item = result,
                    Message = component.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Component>
                {
                    Success = false,
                    Item = component,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportComponentsAsync(int appId, Component[] items)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Component[] validatedItems = ValidateComponents(components: items, parameterName: "items")
            .ToArray();

        string[] names = validatedItems.Select(selector: component => component.Name.ToLower())
            .ToArray();

        var dbVersions = processingService.GetAllComponent()
            .Where(predicate: component => component.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: component.Name.ToLower()))
            .Select(selector: component => new { component.Id, component.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: component =>
        {
            component.AppId = appId;

            component.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: component.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdateComponentResult(newComponent: validatedItems);
    }

    public async ValueTask DeleteAllComponentAsync(IEnumerable<Component> deletedComponent)
    {
        Component[] components = ValidateComponents(components: deletedComponent, parameterName: "items")
            .ToArray();

        foreach (Component component in components)
        {
            await DeleteAsync(componentId: component.Id);
        }
    }

    private static void ValidateId(int componentId, string parameterName) =>
        ThrowIf(condition: componentId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static Component ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return component;
    }

    private static IEnumerable<Component> ValidateComponents(IEnumerable<Component> components, string parameterName)
    {
        if (components == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return components;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}