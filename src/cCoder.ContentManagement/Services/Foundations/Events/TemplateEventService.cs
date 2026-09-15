// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class TemplateEventService(ITemplateEventBroker templateEventBroker) : ITemplateEventService
{
    public ValueTask RaiseTemplateAddEventAsync(Template template, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateAddEventAsync(inputs: [template, userId]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = template
        };

        await templateEventBroker.RaiseTemplateAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseTemplateUpdateEventAsync(Template template, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateUpdateEventAsync(inputs: [template, userId]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = template
        };

        await templateEventBroker.RaiseTemplateUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseTemplateDeleteEventAsync(Template template, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseTemplateDeleteEventAsync(inputs: [template, userId]);

        EventMessage<Template> message = new EventMessage<Template>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = template
        };

        await templateEventBroker.RaiseTemplateDeleteEventAsync(message: message);

    }, isValueTask: true);
}