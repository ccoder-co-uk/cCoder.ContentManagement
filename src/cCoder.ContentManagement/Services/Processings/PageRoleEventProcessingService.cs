// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRoleEventProcessingService(IPageRoleEventService eventService) : IPageRoleEventProcessingService
{
    public ValueTask RaisePageRoleAddEventAsync(PageRole pageRole) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageRoleAddEventAsync(inputs: [pageRole]);
        ValidatePageRole(pageRole: pageRole, parameterName: "entity");

        return eventService.RaisePageRoleAddEventAsync(entity: pageRole);

    }, isValueTask: true);

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole pageRole) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePageRoleDeleteEventAsync(inputs: [pageRole]);
        ValidatePageRole(pageRole: pageRole, parameterName: "entity");

        return eventService.RaisePageRoleDeleteEventAsync(entity: pageRole);

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