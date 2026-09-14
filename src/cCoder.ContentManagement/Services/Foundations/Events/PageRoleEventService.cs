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
    public ValueTask RaisePageRoleAddEventAsync(PageRole pageRole, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageRoleAddEventAsync(inputs: [pageRole, userId]);

        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = pageRole
        };

        await pageRoleEventBroker.RaisePageRoleAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaisePageRoleDeleteEventAsync(PageRole pageRole, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaisePageRoleDeleteEventAsync(inputs: [pageRole, userId]);

        EventMessage<PageRole> message = new EventMessage<PageRole>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = pageRole
        };

        await pageRoleEventBroker.RaisePageRoleDeleteEventAsync(message: message);

    }, isValueTask: true);
}