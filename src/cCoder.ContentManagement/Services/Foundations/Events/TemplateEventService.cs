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
    public ValueTask RaiseTemplateAddEventAsync(Template entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateAddEventAsync(inputs: [entity]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await templateEventBroker.RaiseTemplateAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseTemplateUpdateEventAsync(Template entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateUpdateEventAsync(inputs: [entity]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await templateEventBroker.RaiseTemplateUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseTemplateDeleteEventAsync(Template entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateDeleteEventAsync(inputs: [entity]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await templateEventBroker.RaiseTemplateDeleteEventAsync(message: message);

    }, isValueTask: true);
}