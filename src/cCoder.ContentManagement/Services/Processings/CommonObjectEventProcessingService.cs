using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Processings;

internal class CommonObjectEventProcessingService(ICommonObjectEventService eventService) : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity)
    {
        return eventService.RaiseCommonObjectAddEventAsync(ValidateCommonObject(entity, "entity"));
    }

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity)
    {
        return eventService.RaiseCommonObjectUpdateEventAsync(ValidateCommonObject(entity, "entity"));
    }

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity)
    {
        return eventService.RaiseCommonObjectDeleteEventAsync(ValidateCommonObject(entity, "entity"));
    }

    private static CommonObject ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return commonObject;
    }
}
