using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Processings;

internal class ContentEventProcessingService(IContentEventService eventService) : IContentEventProcessingService
{
    public ValueTask RaiseContentAddEventAsync(Content entity)
    {
        return eventService.RaiseContentAddEventAsync(ValidateContent(entity, "entity"));
    }

    public ValueTask RaiseContentUpdateEventAsync(Content entity)
    {
        return eventService.RaiseContentUpdateEventAsync(ValidateContent(entity, "entity"));
    }

    public ValueTask RaiseContentDeleteEventAsync(Content entity)
    {
        return eventService.RaiseContentDeleteEventAsync(ValidateContent(entity, "entity"));
    }

    private static Content ValidateContent(Content content, string parameterName)
    {
        if (content == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return content;
    }
}
