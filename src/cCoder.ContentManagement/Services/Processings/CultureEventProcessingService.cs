using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Processings;

internal class CultureEventProcessingService(ICultureEventService eventService) : ICultureEventProcessingService
{
    public ValueTask RaiseCultureAddEventAsync(Culture entity)
    {
        return eventService.RaiseCultureAddEventAsync(ValidateCulture(entity, "entity"));
    }

    public ValueTask RaiseCultureUpdateEventAsync(Culture entity)
    {
        return eventService.RaiseCultureUpdateEventAsync(ValidateCulture(entity, "entity"));
    }

    public ValueTask RaiseCultureDeleteEventAsync(Culture entity)
    {
        return eventService.RaiseCultureDeleteEventAsync(ValidateCulture(entity, "entity"));
    }

    private static Culture ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return culture;
    }
}
