using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ICultureEventService
{
    ValueTask RaiseCultureAddEventAsync(Culture entity);

    ValueTask RaiseCultureUpdateEventAsync(Culture entity);

    ValueTask RaiseCultureDeleteEventAsync(Culture entity);
}
