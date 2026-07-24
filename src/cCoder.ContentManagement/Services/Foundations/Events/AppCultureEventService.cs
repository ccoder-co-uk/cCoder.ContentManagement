// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppCultureEventService(IAppCultureEventBroker appCultureEventBroker, ICoreAuthInfo authInfo) : IAppCultureEventService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppCultureAddEventAsync(inputs: [entity]);

        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await appCultureEventBroker.RaiseAppCultureAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppCultureDeleteEventAsync(inputs: [entity]);

        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await appCultureEventBroker.RaiseAppCultureDeleteEventAsync(message: message);

    }, isValueTask: true);
}