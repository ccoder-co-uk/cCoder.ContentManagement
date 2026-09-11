// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageInfoEventProcessingService(IPageInfoEventService eventService) : IPageInfoEventProcessingService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoAddEventAsync(inputs: [pageInfo]);
        ValidatePageInfo(pageInfo: pageInfo, parameterName: "entity");

        return eventService.RaisePageInfoAddEventAsync(entity: pageInfo);

    }, isValueTask: true);

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoUpdateEventAsync(inputs: [pageInfo]);
        ValidatePageInfo(pageInfo: pageInfo, parameterName: "entity");

        return eventService.RaisePageInfoUpdateEventAsync(entity: pageInfo);

    }, isValueTask: true);

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo pageInfo) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoDeleteEventAsync(inputs: [pageInfo]);
        ValidatePageInfo(pageInfo: pageInfo, parameterName: "entity");

        return eventService.RaisePageInfoDeleteEventAsync(entity: pageInfo);

    }, isValueTask: true);

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(condition: pageInfo == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}