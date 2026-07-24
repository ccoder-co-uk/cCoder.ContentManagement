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
    public Component Get(int id) =>
        service.Get(id: id);

    public IQueryable<Component> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Component> AddAsync(Component entity) =>
        service.AddAsync(component: entity);

    public ValueTask<Component> UpdateAsync(Component entity) =>
        service.UpdateAsync(component: entity);

    public ValueTask DeleteAsync(int id) =>
        service.DeleteAsync(id: id);

    public async ValueTask<IEnumerable<Result<Component>>> AddOrUpdate(IEnumerable<Component> items)
    {
        ValidateComponents(components: items, parameterName: "items");
        List<Result<Component>> results = new List<Result<Component>>();

        foreach (Component item in items)
        {
            try
            {
                Component savedItem = item.Id < 1 ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

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

    public async ValueTask DeleteAllAsync(IEnumerable<Component> items)
    {
        ValidateComponents(components: items, parameterName: "items");

        foreach (Component item in items)
        {
            await DeleteAsync(id: item.Id);
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