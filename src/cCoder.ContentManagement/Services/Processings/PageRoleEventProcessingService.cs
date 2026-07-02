using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageRoleEventProcessingService(IPageRoleEventService eventService) : IPageRoleEventProcessingService
{
    public ValueTask RaisePageRoleAddEventAsync(PageRole entity)
    {
        ValidatePageRole(entity, "entity");

        return eventService.RaisePageRoleAddEventAsync(entity);
    }

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole entity)
    {
        ValidatePageRole(entity, "entity");

        return eventService.RaisePageRoleDeleteEventAsync(entity);
    }

    private static void ValidatePageRole(PageRole pageRole, string parameterName) =>
        ThrowIf(pageRole == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
