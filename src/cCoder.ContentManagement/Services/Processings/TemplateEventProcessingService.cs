// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateEventProcessingService(ITemplateEventService eventService) : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template template) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateAddEventAsync(inputs: [template]);
        ValidateTemplate(template: template, parameterName: "entity");

        return eventService.RaiseTemplateAddEventAsync(entity: template);

    }, isValueTask: true);

    public ValueTask RaiseTemplateUpdateEventAsync(Template template) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateUpdateEventAsync(inputs: [template]);
        ValidateTemplate(template: template, parameterName: "entity");

        return eventService.RaiseTemplateUpdateEventAsync(entity: template);

    }, isValueTask: true);

    public ValueTask RaiseTemplateDeleteEventAsync(Template template) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateDeleteEventAsync(inputs: [template]);
        ValidateTemplate(template: template, parameterName: "entity");

        return eventService.RaiseTemplateDeleteEventAsync(entity: template);

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