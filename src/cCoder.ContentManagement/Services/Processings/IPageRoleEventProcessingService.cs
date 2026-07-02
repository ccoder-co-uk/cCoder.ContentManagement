using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageRoleEventProcessingService
{
    ValueTask RaisePageRoleAddEventAsync(PageRole entity);

    ValueTask RaisePageRoleDeleteEventAsync(PageRole entity);
}
