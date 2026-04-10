using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateEventProcessingService(ITemplateEventService eventService) : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template entity)
    {
        return eventService.RaiseTemplateAddEventAsync(ValidateTemplate(entity, "entity"));
    }

    public ValueTask RaiseTemplateUpdateEventAsync(Template entity)
    {
        return eventService.RaiseTemplateUpdateEventAsync(ValidateTemplate(entity, "entity"));
    }

    public ValueTask RaiseTemplateDeleteEventAsync(Template entity)
    {
        return eventService.RaiseTemplateDeleteEventAsync(ValidateTemplate(entity, "entity"));
    }

    private static Template ValidateTemplate(Template template, string parameterName)
    {
        if (template == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return template;
    }
}
