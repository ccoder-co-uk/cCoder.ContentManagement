using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;

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
        await appCultureEventBroker.RaiseAppCultureAddEventAsync(message);
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
        await appCultureEventBroker.RaiseAppCultureDeleteEventAsync(message);
    }
}
