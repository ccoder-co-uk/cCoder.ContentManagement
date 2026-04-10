using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ICommonObjectEventService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity);
}
