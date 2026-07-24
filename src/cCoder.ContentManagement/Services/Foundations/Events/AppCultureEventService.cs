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
    public async ValueTask RaiseAppCultureAddEventAsync(AppCulture entity)
    {
        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await appCultureEventBroker.RaiseAppCultureAddEventAsync(message: message);
    }

    public async ValueTask RaiseAppCultureDeleteEventAsync(AppCulture entity)
    {
        EventMessage<AppCulture> message = new EventMessage<AppCulture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await appCultureEventBroker.RaiseAppCultureDeleteEventAsync(message: message);
    }
}