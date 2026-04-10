using Resource = cCoder.Data.Models.CMS.Resource;

namespace cCoder.ContentManagement.Services.Processings;

public interface IResourceEventProcessingService
{
    ValueTask RaiseResourceAddEventAsync(Resource entity);

    ValueTask RaiseResourceUpdateEventAsync(Resource entity);

    ValueTask RaiseResourceDeleteEventAsync(Resource entity);
}
