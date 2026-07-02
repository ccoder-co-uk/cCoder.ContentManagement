using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureEventProcessingService(ICultureEventService eventService) : ICultureEventProcessingService
{
    public ValueTask RaiseCultureAddEventAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");

        return eventService.RaiseCultureAddEventAsync(entity);
    }

    public ValueTask RaiseCultureUpdateEventAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");

        return eventService.RaiseCultureUpdateEventAsync(entity);
    }

    public ValueTask RaiseCultureDeleteEventAsync(Culture entity)
    {
        ValidateCulture(entity, "entity");

        return eventService.RaiseCultureDeleteEventAsync(entity);
    }

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
            throw new ValidationException(parameterName + " is required.");

        return culture;
    }
}
