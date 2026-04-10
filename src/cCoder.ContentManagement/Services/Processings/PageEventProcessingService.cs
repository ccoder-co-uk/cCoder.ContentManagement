using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageEventProcessingService(IPageEventService eventService) : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page entity)
    {
        return eventService.RaisePageAddEventAsync(ValidatePage(entity, "entity"));
    }

    public ValueTask RaisePageUpdateEventAsync(Page entity)
    {
        return eventService.RaisePageUpdateEventAsync(ValidatePage(entity, "entity"));
    }

    public ValueTask RaisePageDeleteEventAsync(Page entity)
    {
        return eventService.RaisePageDeleteEventAsync(ValidatePage(entity, "entity"));
    }

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return page;
    }
}
