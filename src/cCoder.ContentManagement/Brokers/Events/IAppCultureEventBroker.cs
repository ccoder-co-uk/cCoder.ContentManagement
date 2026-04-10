using cCoder.Data.Models.CMS;
using EventLibrary.Models;

namespace cCoder.ContentManagement.Brokers.Events;

public interface IAppCultureEventBroker
{
    ValueTask RaiseAppCultureAddEventAsync(EventMessage<AppCulture> message);

    ValueTask RaiseAppCultureDeleteEventAsync(EventMessage<AppCulture> message);
}
