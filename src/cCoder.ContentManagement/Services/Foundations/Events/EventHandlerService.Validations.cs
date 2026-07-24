// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService
{
    private static IEventHubBroker ValidateEventHubBroker(IEventHubBroker broker, string parameterName)
    {
        if (broker == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return broker;
    }
}