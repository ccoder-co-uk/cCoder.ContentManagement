// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageEventProcessingService(IPageEventService eventService) : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page page) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        return eventService.RaisePageAddEventAsync(entity: page);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page page) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        return eventService.RaisePageUpdateEventAsync(entity: page);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page page) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "entity");

        return eventService.RaisePageDeleteEventAsync(entity: page);

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