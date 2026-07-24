// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class CultureOrchestrationService(
    ICultureProcessingService processingService,
    ICultureEventProcessingService eventService) : ICultureOrchestrationService
{
    public Culture GetCulture(string cultureId) =>
        TryCatch<Culture>(operation: () =>
    {
        ValidateCultureOnGet(inputs: [cultureId]);
        return processingService.GetCulture(cultureId: ValidateId(cultureId: cultureId, parameterName: "id"));
    });

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Culture>>(operation: () =>
    {
        ValidateAllCultureOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllCulture(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Culture> AddCultureAsync(Culture newCulture) =>
        TryCatch<Culture>(operation: async () =>
    {
        ValidateCultureOnAdd(inputs: [newCulture]);
        ValidateCulture(culture: newCulture, parameterName: "entity");

        Culture result = await processingService.AddCultureAsync(newCulture: newCulture);
        await eventService.RaiseCultureAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture) =>
        TryCatch<Culture>(operation: async () =>
    {
        ValidateCultureOnUpdate(inputs: [updatedCulture]);
        ValidateCulture(culture: updatedCulture, parameterName: "entity");

        Culture result = await processingService.UpdateCultureAsync(updatedCulture: updatedCulture);
        await eventService.RaiseCultureUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(string cultureId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [cultureId]);
        ValidateId(cultureId: cultureId, parameterName: "id");

        Culture entity = processingService.GetCulture(cultureId: cultureId);
        await eventService.RaiseCultureDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(cultureId: cultureId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<Culture>>> AddOrUpdateCultureResult(IEnumerable<Culture> newCulture) =>
        TryCatch<IEnumerable<Result<Culture>>>(operation: () =>
    {
        ValidateOrUpdateCultureResultOnAdd(inputs: [newCulture]);
        return processingService.AddOrUpdateCultureResult(newCulture: ValidateCultures(cultures: newCulture, parameterName: "items"));
    }, isValueTask: true);

    public ValueTask DeleteAllCultureAsync(IEnumerable<Culture> deletedCulture) =>
        TryCatch(operation: () =>
    {
        ValidateAllCultureOnDelete(inputs: [deletedCulture]);
        return processingService.DeleteAllCultureAsync(deletedCulture: ValidateCultures(cultures: deletedCulture, parameterName: "items"));
    }, isValueTask: true);

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