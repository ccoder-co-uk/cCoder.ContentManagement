using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface IContentEventProcessingService
{
    ValueTask RaiseContentAddEventAsync(Content entity);

    ValueTask RaiseContentUpdateEventAsync(Content entity);

    ValueTask RaiseContentDeleteEventAsync(Content entity);
}
