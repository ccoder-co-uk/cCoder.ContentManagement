// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureProcessingService(ICultureService service) : ICultureProcessingService
{
    public Culture Get(string id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Culture> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Culture> AddAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");
        return service.AddAsync(culture: entity);
    }

    public ValueTask<Culture> UpdateAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");
        return service.UpdateAsync(culture: entity);
    }

    public ValueTask DeleteAsync(string id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items)
    {
        ValidateCultures(cultures: items, parameterName: "items");
        List<Result<Culture>> results = new List<Result<Culture>>();

        foreach (Culture item in items)
        {
            try
            {
                Culture savedItem = string.IsNullOrWhiteSpace(value: item.Id) ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Culture>
                {
                    Success = true,
                    Item = savedItem,
                    Message = string.IsNullOrWhiteSpace(value: item.Id) ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Culture>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Culture> items)
    {
        ValidateCultures(cultures: items, parameterName: "items");

        foreach (Culture item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(string id, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: id), message: parameterName + " is required.");

    private static void ValidateCulture(Culture culture, string parameterName) =>
        ThrowIf(condition: (object)culture == null, message: parameterName + " is required.");

    private static void ValidateCultures(IEnumerable<Culture> cultures, string parameterName) =>
        ThrowIf(condition: cultures == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}