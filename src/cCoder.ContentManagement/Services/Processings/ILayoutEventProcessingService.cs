using Layout = cCoder.Data.Models.CMS.Layout;

namespace cCoder.ContentManagement.Services.Processings;

public interface ILayoutEventProcessingService
{
    ValueTask RaiseLayoutAddEventAsync(Layout entity);

    ValueTask RaiseLayoutUpdateEventAsync(Layout entity);

    ValueTask RaiseLayoutDeleteEventAsync(Layout entity);
}
