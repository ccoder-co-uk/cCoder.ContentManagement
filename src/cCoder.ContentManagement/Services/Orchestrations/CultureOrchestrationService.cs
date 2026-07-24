// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class CultureOrchestrationService(
    ICultureProcessingService processingService,
    ICultureEventProcessingService eventService) : ICultureOrchestrationService
{
    public Culture GetCulture(string cultureId) =>
        processingService.GetCulture(cultureId: ValidateId(cultureId: cultureId, parameterName: "id"));

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        processingService.GetAllCulture(ignoreFilters: ignoreFilters);

    public async ValueTask<Culture> AddCultureAsync(Culture newCulture)
    {
        ValidateCulture(culture: newCulture, parameterName: "entity");

        Culture result = await processingService.AddCultureAsync(newCulture: newCulture);
        await eventService.RaiseCultureAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture)
    {
        ValidateCulture(culture: updatedCulture, parameterName: "entity");

        Culture result = await processingService.UpdateCultureAsync(updatedCulture: updatedCulture);
        await eventService.RaiseCultureUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string cultureId)
    {
        ValidateId(cultureId: cultureId, parameterName: "id");

        Culture entity = processingService.GetCulture(cultureId: cultureId);
        await eventService.RaiseCultureDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(cultureId: cultureId);
    }

    public ValueTask<IEnumerable<Result<Culture>>> AddOrUpdateCultureResult(IEnumerable<Culture> newCulture) =>
        processingService.AddOrUpdateCultureResult(newCulture: ValidateCultures(cultures: newCulture, parameterName: "items"));

    public ValueTask DeleteAllCultureAsync(IEnumerable<Culture> deletedCulture) =>
        processingService.DeleteAllCultureAsync(deletedCulture: ValidateCultures(cultures: deletedCulture, parameterName: "items"));

    private static string ValidateId(string cultureId, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value: cultureId))
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return cultureId;
    }

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if (culture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return culture;
    }

    private static IEnumerable<Culture> ValidateCultures(IEnumerable<Culture> cultures, string parameterName)
    {
        if (cultures == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return cultures;
    }
}