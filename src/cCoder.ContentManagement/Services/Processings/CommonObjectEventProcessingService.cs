using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class CommonObjectEventProcessingService(ICommonObjectEventService eventService) : ICommonObjectEventProcessingService
{
    public ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");

        return eventService.RaiseCommonObjectAddEventAsync(entity);
    }

    public ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");

        return eventService.RaiseCommonObjectUpdateEventAsync(entity);
    }

    public ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity)
    {
        ValidateCommonObject(entity, "entity");

        return eventService.RaiseCommonObjectDeleteEventAsync(entity);
    }

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName) =>
        ThrowIf(commonObject == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
