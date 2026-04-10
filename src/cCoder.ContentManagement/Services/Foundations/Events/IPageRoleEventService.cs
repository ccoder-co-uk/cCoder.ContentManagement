using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPageRoleEventService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity);
}
