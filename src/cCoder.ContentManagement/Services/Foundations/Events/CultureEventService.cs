using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using EventLibrary.Models;
using Culture = cCoder.Data.Models.CMS.Culture;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class CultureEventService(ICultureEventBroker cultureEventBroker, ICoreAuthInfo authInfo) : ICultureEventService
{
    public async ValueTask RaiseCultureAddEventAsync(Culture entity)
    {
        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await cultureEventBroker.RaiseCultureAddEventAsync(message);
    }

    public async ValueTask RaiseCultureUpdateEventAsync(Culture entity)
    {
        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await cultureEventBroker.RaiseCultureUpdateEventAsync(message);
    }

    public async ValueTask RaiseCultureDeleteEventAsync(Culture entity)
    {
        EventMessage<Culture> message = new EventMessage<Culture>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };
        await cultureEventBroker.RaiseCultureDeleteEventAsync(message);
    }
}
