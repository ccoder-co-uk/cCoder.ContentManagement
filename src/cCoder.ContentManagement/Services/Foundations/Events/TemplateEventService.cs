using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class TemplateEventService(ITemplateEventBroker templateEventBroker, ICoreAuthInfo authInfo) : ITemplateEventService
{
    public async ValueTask RaiseTemplateAddEventAsync(Template entity)
    {
        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await templateEventBroker.RaiseTemplateAddEventAsync(message);
    }

    public async ValueTask RaiseTemplateUpdateEventAsync(Template entity)
    {
        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await templateEventBroker.RaiseTemplateUpdateEventAsync(message);
    }

    public async ValueTask RaiseTemplateDeleteEventAsync(Template entity)
    {
        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await templateEventBroker.RaiseTemplateDeleteEventAsync(message);
    }
}
