// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageEventProcessingService(IPageEventService eventService) : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageAddEventAsync(entity: entity);
    }

    public ValueTask RaisePageUpdateEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageUpdateEventAsync(entity: entity);
    }

    public ValueTask RaisePageDeleteEventAsync(Page entity)
    {
        ValidatePage(page: entity, parameterName: "entity");

        return eventService.RaisePageDeleteEventAsync(entity: entity);
    }

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