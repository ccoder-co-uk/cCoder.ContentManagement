// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageInfoEventProcessingService(IPageInfoEventService eventService) : IPageInfoEventProcessingService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoAddEventAsync(inputs: [entity]);
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoUpdateEventAsync(inputs: [entity]);
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageInfoDeleteEventAsync(inputs: [entity]);
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoDeleteEventAsync(entity: entity);

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