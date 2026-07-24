// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

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

        await templateEventBroker.RaiseTemplateAddEventAsync(message: message);
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

        await templateEventBroker.RaiseTemplateUpdateEventAsync(message: message);
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

        await templateEventBroker.RaiseTemplateDeleteEventAsync(message: message);
    }
}