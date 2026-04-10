using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Processings;

internal class LayoutEventProcessingService(ILayoutEventService eventService) : ILayoutEventProcessingService
{
    public ValueTask RaiseLayoutAddEventAsync(Layout entity)
    {
        return eventService.RaiseLayoutAddEventAsync(ValidateLayout(entity, "entity"));
    }

    public ValueTask RaiseLayoutUpdateEventAsync(Layout entity)
    {
        return eventService.RaiseLayoutUpdateEventAsync(ValidateLayout(entity, "entity"));
    }

    public ValueTask RaiseLayoutDeleteEventAsync(Layout entity)
    {
        return eventService.RaiseLayoutDeleteEventAsync(ValidateLayout(entity, "entity"));
    }

    private static Layout ValidateLayout(Layout layout, string parameterName)
    {
        if (layout == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return layout;
    }
}
