using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICultureEventProcessingService
{
    ValueTask RaiseCultureAddEventAsync(Culture entity);

    ValueTask RaiseCultureUpdateEventAsync(Culture entity);

    ValueTask RaiseCultureDeleteEventAsync(Culture entity);
}
