using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageInfoEventProcessingService
{
    ValueTask RaisePageInfoAddEventAsync(PageInfo entity);

    ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity);

    ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity);
}
