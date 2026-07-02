using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateEventProcessingService(ITemplateEventService eventService) : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");

        return eventService.RaiseTemplateAddEventAsync(entity);
    }

    public ValueTask RaiseTemplateUpdateEventAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");

        return eventService.RaiseTemplateUpdateEventAsync(entity);
    }

    public ValueTask RaiseTemplateDeleteEventAsync(Template entity)
    {
        ValidateTemplate(entity, "entity");

        return eventService.RaiseTemplateDeleteEventAsync(entity);
    }

    private static void ValidateTemplate(Template template, string parameterName) =>
        ThrowIf(template == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
