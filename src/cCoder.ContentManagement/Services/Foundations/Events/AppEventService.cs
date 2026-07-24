// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppEventService(IAppEventBroker appEventBroker, ICoreAuthInfo authInfo) : IAppEventService
{
    public ValueTask RaiseAppAddEventAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppAddEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };

        await appEventBroker.RaiseAppAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseAppUpdateEventAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppUpdateEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };

        await appEventBroker.RaiseAppUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseAppDeleteEventAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseAppDeleteEventAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };

        await appEventBroker.RaiseAppDeleteEventAsync(message: message);

    }, isValueTask: true);
}