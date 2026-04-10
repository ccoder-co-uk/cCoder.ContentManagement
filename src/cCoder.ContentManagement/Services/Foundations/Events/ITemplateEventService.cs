using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ITemplateEventService
{
    ValueTask RaiseTemplateAddEventAsync(Template entity);

    ValueTask RaiseTemplateUpdateEventAsync(Template entity);

    ValueTask RaiseTemplateDeleteEventAsync(Template entity);
}
