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
        ValidateId(id, "id");
        return processingService.Get(id);
    }

    public IQueryable<Component> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Component> AddAsync(Component entity)
    {
        ValidateComponent(entity, "entity");

        Component result = await processingService.AddAsync(entity);
        await eventService.RaiseComponentAddEventAsync(result);
        return result;
    }

    public async ValueTask<Component> UpdateAsync(Component entity)
    {
        ValidateComponent(entity, "entity");

        Component result = await processingService.UpdateAsync(entity);
        await eventService.RaiseComponentUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Component entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(component => component.Id == id);
        }

        if (entity == null)
            return;

        await eventService.RaiseComponentDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Component[] componentsToDelete = [.. GetAll(ignoreFilters: true).Where(component => component.AppId == appId)];

        foreach (Component component in componentsToDelete)
            await DeleteAsync(component.Id);
    }

    public async ValueTask<IEnumerable<Result<Component>>> AddOrUpdate(IEnumerable<Component> items)
    {
        Component[] components = ValidateComponents(items, "items").ToArray();
        List<Result<Component>> results = new();

        foreach (Component component in components)
        {
            try
            {
                Component result = component.Id <= 0
                    ? await AddAsync(component)
                    : await UpdateAsync(component);

                results.Add(new Result<Component>
                {
                    Success = true,
                    Item = result,
                    Message = component.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Component>
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
        ValidateAppId(appId, "appId");

        Component[] validatedItems = ValidateComponents(items, "items").ToArray();
        string[] names = validatedItems.Select(component => component.Name.ToLower()).ToArray();

        var dbVersions = processingService.GetAll()
            .Where(component => component.AppId == appId && ((ReadOnlySpan<string>)names).Contains(component.Name.ToLower()))
            .Select(component => new { component.Id, component.Name })
            .ToArray();

        Array.ForEach(validatedItems, component =>
        {
            component.AppId = appId;
            component.Id = dbVersions.FirstOrDefault(existing =>
                existing.Name.Equals(component.Name, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Component> items)
    {
        Component[] components = ValidateComponents(items, "items").ToArray();

        foreach (Component component in components)
            await DeleteAsync(component.Id);
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidateAppId(int appId, string parameterName) =>
        ThrowIf(appId < 1, parameterName + " must be greater than 0.");

    private static Component ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
            throw new ValidationException(parameterName + " is required.");

        return component;
    }

    private static IEnumerable<Component> ValidateComponents(IEnumerable<Component> components, string parameterName)
    {
        if (components == null)
            throw new ValidationException(parameterName + " is required.");

        return components;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
