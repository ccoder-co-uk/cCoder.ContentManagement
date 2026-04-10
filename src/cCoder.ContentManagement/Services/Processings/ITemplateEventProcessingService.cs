using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Processings;

public interface ITemplateEventProcessingService
{
    ValueTask RaiseTemplateAddEventAsync(Template entity);

    ValueTask RaiseTemplateUpdateEventAsync(Template entity);

    ValueTask RaiseTemplateDeleteEventAsync(Template entity);
}
