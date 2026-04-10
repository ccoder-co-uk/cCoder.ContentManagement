using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageEventProcessingService
{
    ValueTask RaisePageAddEventAsync(Page entity);

    ValueTask RaisePageUpdateEventAsync(Page entity);

    ValueTask RaisePageDeleteEventAsync(Page entity);
}
