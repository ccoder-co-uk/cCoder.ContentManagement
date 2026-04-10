using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoEventProcessingService(IPageInfoEventService eventService) : IPageInfoEventProcessingService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo entity)
    {
        return eventService.RaisePageInfoAddEventAsync(ValidatePageInfo(entity, "entity"));
    }

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity)
    {
        return eventService.RaisePageInfoUpdateEventAsync(ValidatePageInfo(entity, "entity"));
    }

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity)
    {
        return eventService.RaisePageInfoDeleteEventAsync(ValidatePageInfo(entity, "entity"));
    }

    private static PageInfo ValidatePageInfo(PageInfo pageInfo, string parameterName)
    {
        if (pageInfo == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return pageInfo;
    }
}
