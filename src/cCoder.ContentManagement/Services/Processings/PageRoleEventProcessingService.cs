// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRoleEventProcessingService(IPageRoleEventService eventService) : IPageRoleEventProcessingService
{
    public ValueTask RaisePageRoleAddEventAsync(PageRole entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageRoleAddEventAsync(inputs: [entity]);
        ValidatePageRole(pageRole: entity, parameterName: "entity");

        return eventService.RaisePageRoleAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageRoleDeleteEventAsync(inputs: [entity]);
        ValidatePageRole(pageRole: entity, parameterName: "entity");

        return eventService.RaisePageRoleDeleteEventAsync(entity: entity);

    }, isValueTask: true);

    private static void ValidatePageRole(PageRole pageRole, string parameterName) =>
        ThrowIf(condition: pageRole == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}