// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureEventProcessingService(ICultureEventService eventService) : ICultureEventProcessingService
{
    public ValueTask RaiseCultureAddEventAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureAddEventAsync(entity: entity);
    }

    public ValueTask RaiseCultureUpdateEventAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseCultureDeleteEventAsync(Culture entity)
    {
        ValidateCulture(culture: entity, parameterName: "entity");

        return eventService.RaiseCultureDeleteEventAsync(entity: entity);
    }

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return culture;
    }
}