// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageEventProcessingService(IPageEventService eventService) : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [entity]);
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageDeleteEventAsync(entity: entity);

    }, isValueTask: true);

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(condition: page == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}