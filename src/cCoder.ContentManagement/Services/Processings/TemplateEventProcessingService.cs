// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateEventProcessingService(ITemplateEventService eventService) : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateAddEventAsync(inputs: [entity]);
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseTemplateUpdateEventAsync(Template entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateUpdateEventAsync(inputs: [entity]);
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseTemplateDeleteEventAsync(Template entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateDeleteEventAsync(inputs: [entity]);
        ValidateTemplate(template: entity, parameterName: "entity");

        return eventService.RaiseTemplateDeleteEventAsync(entity: entity);

    }, isValueTask: true);

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