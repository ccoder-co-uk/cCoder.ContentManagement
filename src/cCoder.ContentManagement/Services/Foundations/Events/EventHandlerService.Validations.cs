using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService
{
    private static IEventHubBroker ValidateEventHubBroker(IEventHubBroker broker, string parameterName)
    {
        if (broker == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return broker;
    }
}
