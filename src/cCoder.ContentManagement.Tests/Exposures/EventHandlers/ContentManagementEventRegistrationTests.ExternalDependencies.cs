// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.Eventing;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.EventHandlers;

public sealed partial class ContentManagementEventRegistrationTests
{
    [Fact]
    public void EventHandlerExposures_WhenConstructed_ShouldNotDependDirectlyOnExternalEventHub()
    {
        // Given
        Type eventHandlersContract = typeof(IContentManagementEventHandlers);

        Type[] eventHandlerExposures = eventHandlersContract.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass &&
                !type.IsAbstract &&
                eventHandlersContract.IsAssignableFrom(c: type))
            .ToArray();

        // When
        Type[] eventHandlerExposuresDependingOnExternalEventHub = eventHandlerExposures
            .Where(predicate: type => type
                .GetConstructors()
                .SelectMany(selector: constructor => constructor.GetParameters())
                .Any(predicate: parameter => parameter.ParameterType == typeof(IEventHub)))
            .ToArray();

        // Then
        eventHandlerExposuresDependingOnExternalEventHub
            .Should()
            .BeEmpty(because:
                "event-handler exposures must register events through a ContentManagement broker");
    }
}