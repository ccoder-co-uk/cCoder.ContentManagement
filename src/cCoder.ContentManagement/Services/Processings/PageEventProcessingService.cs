// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageEventProcessingService(IPageEventService eventService)
    : IPageEventProcessingService
{
    public ValueTask RaisePageAddEventAsync(Page page, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageAddEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");


        return eventService.RaisePageAddEventAsync(
            entity: page,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePageUpdateEventAsync(Page page, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageUpdateEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");


        return eventService.RaisePageUpdateEventAsync(
            entity: page,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePageDeleteEventAsync(Page page, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageDeleteEventAsync(inputs: [page, userId]);
        ValidatePage(page: page, parameterName: "entity");


        return eventService.RaisePageDeleteEventAsync(
            entity: page,
            userId: userId);

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