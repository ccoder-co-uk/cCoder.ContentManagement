using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICommonObjectEventProcessingService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity);
}
