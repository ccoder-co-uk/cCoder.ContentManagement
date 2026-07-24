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
    public Culture GetCulture(string cultureId)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");
        return service.GetCulture(cultureId: cultureId);
    }

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        service.GetAllCulture(ignoreFilters: ignoreFilters);

    public ValueTask<Culture> AddCultureAsync(Culture newCulture)
    {
        ValidateCulture(culture: newCulture, parameterName: "entity");
        return service.AddCultureAsync(newCulture: newCulture);
    }

    public ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture)
    {
        ValidateCulture(culture: updatedCulture, parameterName: "entity");
        return service.UpdateCultureAsync(updatedCulture: updatedCulture);
    }

    public ValueTask DeleteAsync(string cultureId)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");
        return service.DeleteAsync(cultureId: cultureId);
    }

    public async ValueTask<IEnumerable<Result<Culture>>> AddOrUpdateCultureResult(IEnumerable<Culture> newCulture)
    {
        ValidateCultures(cultures: newCulture, parameterName: "items");
        List<Result<Culture>> results = new List<Result<Culture>>();

        foreach (Culture item in newCulture)
        {
            try
            {
                Culture savedItem = string.IsNullOrWhiteSpace(value: item.Id) ? await ExecuteAddCultureAsync(newCulture: item) : await ExecuteUpdateCultureAsync(updatedCulture: item);

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

    public async ValueTask DeleteAllCultureAsync(IEnumerable<Culture> deletedCulture)
    {
        ValidateCultures(cultures: deletedCulture, parameterName: "items");

        foreach (Culture item in deletedCulture)
        {
            await ExecuteDeleteAsync(cultureId: item.Id);
        }
    }

    private static void ValidateId(string cultureId, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: cultureId), message: parameterName + " is required.");

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

    private ValueTask<Culture> ExecuteAddCultureAsync(Culture newCulture)
    {
        ValidateCulture(culture: newCulture, parameterName: "entity");
        return service.AddCultureAsync(newCulture: newCulture);
    }

    private ValueTask ExecuteDeleteAsync(string cultureId)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");
        return service.DeleteAsync(cultureId: cultureId);
    }

    private ValueTask<Culture> ExecuteUpdateCultureAsync(Culture updatedCulture)
    {
        ValidateCulture(culture: updatedCulture, parameterName: "entity");
        return service.UpdateCultureAsync(updatedCulture: updatedCulture);
    }
}