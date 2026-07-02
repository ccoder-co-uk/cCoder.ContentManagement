using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoEventProcessingService(IPageInfoEventService eventService) : IPageInfoEventProcessingService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");

        return eventService.RaisePageInfoAddEventAsync(entity);
    }

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");

        return eventService.RaisePageInfoUpdateEventAsync(entity);
    }

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");

        return eventService.RaisePageInfoDeleteEventAsync(entity);
    }

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(pageInfo == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
