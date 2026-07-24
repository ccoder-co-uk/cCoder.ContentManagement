// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoEventProcessingService(IPageInfoEventService eventService) : IPageInfoEventProcessingService
{
    public ValueTask RaisePageInfoAddEventAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoAddEventAsync(entity: entity);
    }

    public ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoUpdateEventAsync(entity: entity);
    }

    public ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");

        return eventService.RaisePageInfoDeleteEventAsync(entity: entity);
    }

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