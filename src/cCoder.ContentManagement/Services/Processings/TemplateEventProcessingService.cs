// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateEventProcessingService(ITemplateEventService eventService)
    : ITemplateEventProcessingService
{
    public ValueTask RaiseTemplateAddEventAsync(Template template, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateAddEventAsync(inputs: [template, userId]);
        ValidateTemplate(template: template, parameterName: "entity");


        return eventService.RaiseTemplateAddEventAsync(
            entity: template,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseTemplateUpdateEventAsync(Template template, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateUpdateEventAsync(inputs: [template, userId]);
        ValidateTemplate(template: template, parameterName: "entity");


        return eventService.RaiseTemplateUpdateEventAsync(
            entity: template,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseTemplateDeleteEventAsync(Template template, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseTemplateDeleteEventAsync(inputs: [template, userId]);
        ValidateTemplate(template: template, parameterName: "entity");


        return eventService.RaiseTemplateDeleteEventAsync(
            entity: template,
            userId: userId);

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