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
    public Component Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.Get(id: id);
    }

    public IQueryable<Component> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Component> AddAsync(Component entity)
    {
        ValidateComponent(component: entity, parameterName: "entity");

        Component result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseComponentAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Component> UpdateAsync(Component entity)
    {
        ValidateComponent(component: entity, parameterName: "entity");

        Component result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseComponentUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        Component entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: component => component.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseComponentDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId: appId, parameterName: "appId");

        Component[] componentsToDelete = [.. GetAll(ignoreFilters: true)
            .Where(predicate: component => component.AppId == appId)];

        foreach (Component component in componentsToDelete)
        {
            await DeleteAsync(id: component.Id);
        }
    }

    public async ValueTask<IEnumerable<Result<Component>>> AddOrUpdate(IEnumerable<Component> items)
    {
        Component[] components = ValidateComponents(components: items, parameterName: "items")
            .ToArray();

        List<Result<Component>> results = new();

        foreach (Component component in components)
        {
            try
            {
                Component result = component.Id <= 0
                    ? await AddAsync(entity: component)
                    : await UpdateAsync(entity: component);

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

        var dbVersions = processingService.GetAll()
            .Where(predicate: component => component.AppId == appId && ((ReadOnlySpan<string>)names).Contains(value: component.Name.ToLower()))
            .Select(selector: component => new { component.Id, component.Name })
            .ToArray();

        Array.ForEach(array: validatedItems, action: component =>
        {
            component.AppId = appId;

            component.Id = dbVersions.FirstOrDefault(predicate: existing =>
                existing.Name.Equals(value: component.Name, comparisonType: StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(items: validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Component> items)
    {
        Component[] components = ValidateComponents(components: items, parameterName: "items")
            .ToArray();

        foreach (Component component in components)
        {
            await DeleteAsync(id: component.Id);
        }
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

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