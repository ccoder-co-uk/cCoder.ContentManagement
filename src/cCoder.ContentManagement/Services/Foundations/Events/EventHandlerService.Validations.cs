// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Dependencies;
using System.ComponentModel.DataAnnotations;

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

    private static void ValidateListenToAllEvents(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateListenToFinalAppDeleteEvent(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}