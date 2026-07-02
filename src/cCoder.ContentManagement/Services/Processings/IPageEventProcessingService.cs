using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface IPageEventProcessingService
{
    ValueTask RaisePageAddEventAsync(Page entity);

    ValueTask RaisePageUpdateEventAsync(Page entity);

    ValueTask RaisePageDeleteEventAsync(Page entity);
}
