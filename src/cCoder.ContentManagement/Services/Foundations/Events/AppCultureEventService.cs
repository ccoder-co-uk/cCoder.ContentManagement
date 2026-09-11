// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppCultureEventService(IAppCultureEventBroker appCultureEventBroker) : IAppCultureEventService
{
    public ValueTask RaiseAppCultureAddEventAsync(AppCulture appCulture) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppCultureAddEventAsync(inputs: [appCulture]);

        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = appCultureEventBroker.GetCurrentUserId()
            },
            Data = appCulture
        };

        await appCultureEventBroker.RaiseAppCultureAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseAppCultureDeleteEventAsync(AppCulture appCulture) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppCultureDeleteEventAsync(inputs: [appCulture]);

        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = appCultureEventBroker.GetCurrentUserId()
            },
            Data = appCulture
        };

        await appCultureEventBroker.RaiseAppCultureDeleteEventAsync(message: message);

    }, isValueTask: true);
}