using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ILayoutEventService
{
    ValueTask RaiseLayoutAddEventAsync(Layout entity);

    ValueTask RaiseLayoutUpdateEventAsync(Layout entity);

    ValueTask RaiseLayoutDeleteEventAsync(Layout entity);
}
