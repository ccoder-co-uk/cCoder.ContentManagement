using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Processings;
using Component = cCoder.Data.Models.CMS.Component;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Component>;

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
        {
            return;
        }

        await eventService.RaiseComponentDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Component[] componentsToDelete = [.. GetAll(ignoreFilters: true).Where(component => component.AppId == appId)];

        foreach (Component component in componentsToDelete)
        {
            await DeleteAsync(component.Id);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Component> items)
    {
        Component[] components = ValidateComponents(items, "items").ToArray();
        List<Result> results = new();

        foreach (Component component in components)
        {
            try
            {
                Component result = component.Id <= 0
                    ? await AddAsync(component)
                    : await UpdateAsync(component);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = component.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
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

        var dbVersions = (from component in processingService.GetAll()
                          where component.AppId == appId && ((ReadOnlySpan<string>)names).Contains(component.Name.ToLower())
                          select new { component.Id, component.Name }).ToArray();

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
        {
            await DeleteAsync(component.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static Component ValidateComponent(Component component, string parameterName)
    {
        if (component == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return component;
    }

    private static IEnumerable<Component> ValidateComponents(IEnumerable<Component> components, string parameterName)
    {
        if (components == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return components;
    }
}
