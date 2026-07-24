// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class ComponentProcessingService(IComponentService service) : IComponentProcessingService
{
    public Component GetComponent(int componentId) =>
        service.GetComponent(componentId: componentId);

    public IQueryable<Component> GetAllComponent(bool ignoreFilters = false) =>
        service.GetAllComponent(ignoreFilters: ignoreFilters);

    public ValueTask<Component> AddComponentAsync(Component newComponent) =>
        service.AddComponentAsync(newComponent: newComponent);

    public ValueTask<Component> UpdateComponentAsync(Component updatedComponent) =>
        service.UpdateComponentAsync(updatedComponent: updatedComponent);

    public ValueTask DeleteAsync(int componentId) =>
        service.DeleteAsync(componentId: componentId);

    public async ValueTask<IEnumerable<Result<Component>>> AddOrUpdateComponentResult(IEnumerable<Component> newComponent)
    {
        ValidateComponents(components: newComponent, parameterName: "items");
        List<Result<Component>> results = new List<Result<Component>>();

        foreach (Component item in newComponent)
        {
            try
            {
                Component savedItem = item.Id < 1 ? await AddComponentAsync(newComponent: item) : await UpdateComponentAsync(updatedComponent: item);

                results.Add(item: new Result<Component>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Component>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllComponentAsync(IEnumerable<Component> deletedComponent)
    {
        ValidateComponents(components: deletedComponent, parameterName: "items");

        foreach (Component item in deletedComponent)
        {
            await DeleteAsync(componentId: item.Id);
        }
    }

    private static void ValidateComponents(IEnumerable<Component> components, string parameterName) =>
        ThrowIf(condition: components == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}