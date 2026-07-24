// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class TemplateEventProcessingService(ITemplateEventService eventService) : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateAddEventAsync(entity: entity);
    }

    public ValueTask RaiseTemplateUpdateEventAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseTemplateDeleteEventAsync(Template entity)
    {
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateDeleteEventAsync(entity: entity);
    }

    private static void ValidateTemplate(Template template, string parameterName) =>
        ThrowIf(condition: template == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}