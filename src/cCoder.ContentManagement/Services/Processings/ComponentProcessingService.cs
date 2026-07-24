// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ComponentProcessingService(IComponentService service) : IComponentProcessingService
{
    public Component GetComponent(int componentId) =>
        TryCatch<Component>(operation: () =>
    {
        ValidateComponentOnGet(inputs: [componentId]);
        return service.GetComponent(componentId: componentId);
    });

    public IQueryable<Component> GetAllComponent(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Component>>(operation: () =>
    {
        ValidateAllComponentOnGet(inputs: [ignoreFilters]);
        return service.GetAllComponent(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Component> AddComponentAsync(Component newComponent) =>
        TryCatch<Component>(operation: () =>
    {
        ValidateComponentOnAdd(inputs: [newComponent]);
        return service.AddComponentAsync(newComponent: newComponent);
    }, isValueTask: true);

    public ValueTask<Component> UpdateComponentAsync(Component updatedComponent) =>
        TryCatch<Component>(operation: () =>
    {
        ValidateComponentOnUpdate(inputs: [updatedComponent]);
        return service.UpdateComponentAsync(updatedComponent: updatedComponent);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int componentId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [componentId]);
        return service.DeleteAsync(componentId: componentId);
    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Component>>> AddOrUpdateComponentResult(IEnumerable<Component> newComponent) =>
        TryCatch<IEnumerable<OperationResult<Component>>>(operation: async () =>
    {
        ValidateOrUpdateComponentResultOnAdd(inputs: [newComponent]);
        ValidateComponents(components: newComponent, parameterName: "items");
        List<OperationResult<Component>> results = new List<OperationResult<Component>>();

        foreach (Component item in newComponent)
        {
            try
            {
                Component savedItem = item.Id < 1 ? await ExecuteAddComponentAsync(newComponent: item) : await ExecuteUpdateComponentAsync(updatedComponent: item);

                results.Add(item: new OperationResult<Component>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Component>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllComponentAsync(IEnumerable<Component> deletedComponent) =>
        TryCatch(operation: async () =>
    {
        ValidateAllComponentOnDelete(inputs: [deletedComponent]);
        ValidateComponents(components: deletedComponent, parameterName: "items");

        foreach (Component item in deletedComponent)
        {
            await ExecuteDeleteAsync(componentId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateComponents(IEnumerable<Component> components, string parameterName) =>
        ThrowIf(condition: components == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private ValueTask<Component> ExecuteAddComponentAsync(Component newComponent) =>
        service.AddComponentAsync(newComponent: newComponent);

    private ValueTask ExecuteDeleteAsync(int componentId) =>
        service.DeleteAsync(componentId: componentId);

    private ValueTask<Component> ExecuteUpdateComponentAsync(Component updatedComponent) =>
        service.UpdateComponentAsync(updatedComponent: updatedComponent);
}