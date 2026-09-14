// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class CultureEventProcessingService(ICultureEventService eventService)
    : ICultureEventProcessingService
{
    public ValueTask RaiseCultureAddEventAsync(Culture culture, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureAddEventAsync(inputs: [culture, userId]);
        ValidateCulture(culture: culture, parameterName: "entity");


        return eventService.RaiseCultureAddEventAsync(
            entity: culture,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseCultureUpdateEventAsync(Culture culture, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureUpdateEventAsync(inputs: [culture, userId]);
        ValidateCulture(culture: culture, parameterName: "entity");


        return eventService.RaiseCultureUpdateEventAsync(
            entity: culture,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseCultureDeleteEventAsync(Culture culture, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseCultureDeleteEventAsync(inputs: [culture, userId]);
        ValidateCulture(culture: culture, parameterName: "entity");


        return eventService.RaiseCultureDeleteEventAsync(
            entity: culture,
            userId: userId);

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