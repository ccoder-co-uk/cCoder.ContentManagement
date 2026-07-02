using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutEventProcessingService(ILayoutEventService eventService) : ILayoutEventProcessingService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");

        return eventService.RaiseLayoutAddEventAsync(entity);
    }

    public ValueTask RaiseLayoutUpdateEventAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");

        return eventService.RaiseLayoutUpdateEventAsync(entity);
    }

    public ValueTask RaiseLayoutDeleteEventAsync(Layout entity)
    {
        ValidateLayout(entity, "entity");

        return eventService.RaiseLayoutDeleteEventAsync(entity);
    }

    private static void ValidateLayout(Layout layout, string parameterName) =>
        ThrowIf(layout == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
