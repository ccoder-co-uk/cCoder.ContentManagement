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
    public Culture Get(string id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Culture> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Culture> AddAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");

        Culture result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseCultureAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Culture> UpdateAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");

        Culture result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseCultureUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        ValidateId(id: id, parameterName: "id");

        Culture entity = processingService.Get(id: id);
        await eventService.RaiseCultureDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Culture>>> AddOrUpdate(IEnumerable<Culture> items) =>
        processingService.AddOrUpdate(items: ValidateCultures(cultures: items, parameterName: "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Culture> items) =>
        processingService.DeleteAllAsync(items: ValidateCultures(cultures: items, parameterName: "items"));

    private static string ValidateId(string id, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value: id))
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return id;
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