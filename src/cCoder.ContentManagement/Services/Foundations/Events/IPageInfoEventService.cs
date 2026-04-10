using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPageInfoEventService
{
    ValueTask RaisePageInfoAddEventAsync(PageInfo entity);

    ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity);

    ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity);
}
