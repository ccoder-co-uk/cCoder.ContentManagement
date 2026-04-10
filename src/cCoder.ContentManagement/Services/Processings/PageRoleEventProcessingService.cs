using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageRoleEventProcessingService(IPageRoleEventService eventService) : IPageRoleEventProcessingService
{
    public ValueTask RaisePageRoleAddEventAsync(PageRole entity)
    {
        return eventService.RaisePageRoleAddEventAsync(ValidatePageRole(entity, "entity"));
    }

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole entity)
    {
        return eventService.RaisePageRoleDeleteEventAsync(ValidatePageRole(entity, "entity"));
    }

    private static PageRole ValidatePageRole(PageRole pageRole, string parameterName)
    {
        if (pageRole == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return pageRole;
    }
}
