using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ContentEventProcessingService(IContentEventService eventService) : IContentEventProcessingService
{
    public ValueTask RaiseContentAddEventAsync(Content entity)
    {
        ValidateContent(entity, "entity");

        return eventService.RaiseContentAddEventAsync(entity);
    }

    public ValueTask RaiseContentUpdateEventAsync(Content entity)
    {
        ValidateContent(entity, "entity");

        return eventService.RaiseContentUpdateEventAsync(entity);
    }

    public ValueTask RaiseContentDeleteEventAsync(Content entity)
    {
        ValidateContent(entity, "entity");

        return eventService.RaiseContentDeleteEventAsync(entity);
    }

    private static void ValidateContent(Content content, string parameterName) =>
        ThrowIf(content == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
