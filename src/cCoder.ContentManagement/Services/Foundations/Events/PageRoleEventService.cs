// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class PageRoleEventService(IPageRoleEventBroker pageRoleEventBroker) : IPageRoleEventService
{
    public ValueTask RaisePageRoleAddEventAsync(PageRole entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageRoleAddEventAsync(inputs: [entity]);

        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageRoleEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageRoleEventBroker.RaisePageRoleAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageRoleDeleteEventAsync(inputs: [entity]);

        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = pageRoleEventBroker.GetCurrentUserId()
            },
            Data = entity
        };

        await pageRoleEventBroker.RaisePageRoleDeleteEventAsync(message: message);

    }, isValueTask: true);
}