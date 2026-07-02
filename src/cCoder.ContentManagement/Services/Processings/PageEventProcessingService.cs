using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageEventProcessingService(IPageEventService eventService) : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");

        return eventService.RaisePageAddEventAsync(entity);
    }

    public ValueTask RaisePageUpdateEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");

        return eventService.RaisePageUpdateEventAsync(entity);
    }

    public ValueTask RaisePageDeleteEventAsync(Page entity)
    {
        ValidatePage(entity, "entity");

        return eventService.RaisePageDeleteEventAsync(entity);
    }

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(page == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
