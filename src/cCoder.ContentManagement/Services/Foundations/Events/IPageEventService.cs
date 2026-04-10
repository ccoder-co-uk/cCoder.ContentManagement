using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPageEventService
{
    ValueTask RaisePageAddEventAsync(Page entity);

    ValueTask RaisePageUpdateEventAsync(Page entity);

    ValueTask RaisePageDeleteEventAsync(Page entity);
}
