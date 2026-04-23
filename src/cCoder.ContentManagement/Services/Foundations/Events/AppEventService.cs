using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class AppEventService(IAppEventBroker appEventBroker, ICoreAuthInfo authInfo) : IAppEventService
{
    public async ValueTask RaiseAppAddEventAsync(App app)
    {
        ValidateApp(app, "app");
        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };
        await appEventBroker.RaiseAppAddEventAsync(message);
    }

    public async ValueTask RaiseAppUpdateEventAsync(App app)
    {
        ValidateApp(app, "app");
        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };
        await appEventBroker.RaiseAppUpdateEventAsync(message);
    }

    public async ValueTask RaiseAppDeleteEventAsync(App app)
    {
        ValidateApp(app, "app");
        EventMessage<App> message = new EventMessage<App>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = app
        };
        await appEventBroker.RaiseAppDeleteEventAsync(message);
    }
}
