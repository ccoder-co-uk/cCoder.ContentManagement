using Content = cCoder.Data.Models.CMS.Content;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IContentEventService
{
    ValueTask RaiseContentAddEventAsync(Content entity);

    ValueTask RaiseContentUpdateEventAsync(Content entity);

    ValueTask RaiseContentDeleteEventAsync(Content entity);
}
