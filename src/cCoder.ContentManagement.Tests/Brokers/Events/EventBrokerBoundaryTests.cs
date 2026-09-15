// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Eventing;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers.Events;

public sealed partial class EventBrokerBoundaryTests
{
    [Fact]
    public void EventBrokers_WhenConstructed_ShouldConsumeEventHubDirectly()
    {
        // Given
        Type eventBrokerType = typeof(IAppEventBroker);

        Type[] concreteEventBrokerTypes = eventBrokerType
            .Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsClass
                && !type.IsAbstract
                && type.Namespace == eventBrokerType.Namespace
                && type.Name.EndsWith(
                    value: "EventBroker",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        // When
        Type[][] constructorParameterTypes = concreteEventBrokerTypes
            .Select(selector: type => type
                .GetConstructors()
                .Single()
                .GetParameters()
                .Select(selector: parameter => parameter.ParameterType)
                .ToArray())
            .ToArray();

        // Then
        concreteEventBrokerTypes.Should()
            .NotBeEmpty();

        constructorParameterTypes.Should()
            .OnlyContain(predicate: parameterTypes =>
            parameterTypes.Contains(value: typeof(IEventHub))
            && !parameterTypes.Contains(value: typeof(IAuthenticatedEventHub)));
    }
}