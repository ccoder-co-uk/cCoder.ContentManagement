// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CultureEventProcessingService(ICultureEventService eventService) : ICultureEventProcessingService
{
    public ValueTask RaiseCultureAddEventAsync(Culture entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureAddEventAsync(inputs: [entity]);
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseCultureUpdateEventAsync(Culture entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureUpdateEventAsync(inputs: [entity]);
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseCultureDeleteEventAsync(Culture entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureDeleteEventAsync(inputs: [entity]);
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureDeleteEventAsync(entity: entity);

    }, isValueTask: true);

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return culture;
    }
}