using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICommonObjectEventProcessingService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity);
}
