// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Eventing;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers.Events;

public sealed class EventBrokerBoundaryTests
{
    [Fact]
    public void EventBrokers_WhenConstructed_ShouldConsumeEventHubDirectly()
    {
        // Given
        Type eventBrokerType = typeof(IAppEventBroker);

        Type[] concreteEventBrokerTypes = eventBrokerType
            .Assembly
            .GetTypes()
            .Where(type =>
                type.IsClass
                && !type.IsAbstract
                && type.Namespace == eventBrokerType.Namespace
                && type.Name.EndsWith(
                    value: "EventBroker",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        // When
        Type[][] constructorParameterTypes = concreteEventBrokerTypes
            .Select(type => type
                .GetConstructors()
                .Single()
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray())
            .ToArray();

        // Then
        concreteEventBrokerTypes.Should().NotBeEmpty();

        constructorParameterTypes.Should().OnlyContain(parameterTypes =>
            parameterTypes.Contains(value: typeof(IEventHub))
            && !parameterTypes.Contains(value: typeof(IAuthenticatedEventHub)));
    }
}