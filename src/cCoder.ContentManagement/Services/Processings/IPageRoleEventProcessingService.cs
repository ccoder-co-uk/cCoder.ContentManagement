using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRoleEventProcessingService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity);
}
