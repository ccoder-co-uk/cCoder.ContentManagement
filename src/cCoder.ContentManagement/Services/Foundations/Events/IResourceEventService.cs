using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IResourceEventService
{
    ValueTask RaiseResourceAddEventAsync(Resource entity);

    ValueTask RaiseResourceUpdateEventAsync(Resource entity);

    ValueTask RaiseResourceDeleteEventAsync(Resource entity);
}
