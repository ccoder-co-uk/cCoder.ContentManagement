// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public partial class ComponentRenderServiceTests
{
    [Fact]
    public void ShouldExecuteNamedComponentRenderServiceOperation()
    {
        // Given
        const string serviceName = "ComponentRender";
        const string expectedResult = "rendered";
        object namedService = new();
        Func<object, string> operation = _ => expectedResult;

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<object>(
                    name: serviceName))
            .Returns(value: namedService);

        // When
        string actualResult =
            componentRenderService.Execute<object, string>(
                name: serviceName,
                operation: operation);

        // Then
        actualResult.Should()
            .Be(expected: expectedResult);

        serviceProviderBrokerMock.Verify(
            expression: broker =>
                broker.GetRequiredService<object>(
                    name: serviceName),
            times: Times.Once);
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenNameIsInvalid()
    {
        // Given
        Func<object, string> operation = _ => string.Empty;

        // When
        Action executeInvalidName = () =>
            componentRenderService.Execute<object, string>(
                name: null!,
                operation: operation);

        // Then
        executeInvalidName.Should()
            .Throw<ContentManagementValidationException>();
    }

    [Fact]
    public void ShouldWrapDependencyExceptionWhenProviderFails()
    {
        // Given
        const string serviceName = "MissingComponentRenderService";
        InvalidOperationException dependencyException = new();
        Func<object, string> operation = _ => string.Empty;

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<object>(
                    name: serviceName))
            .Throws(exception: dependencyException);

        // When
        Action executeMissingService = () =>
            componentRenderService.Execute<object, string>(
                name: serviceName,
                operation: operation);

        // Then
        executeMissingService.Should()
            .Throw<ContentManagementDependencyException>()
            .WithInnerException<InvalidOperationException>()
            .Which.Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapUnclassifiedExceptionWhenOperationFails()
    {
        // Given
        const string serviceName = "FailingComponentRenderService";
        object namedService = new();
        Exception unclassifiedException = new();
        Func<object, string> operation = _ => throw unclassifiedException;

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<object>(
                    name: serviceName))
            .Returns(value: namedService);

        // When
        Action executeFailingOperation = () =>
            componentRenderService.Execute<object, string>(
                name: serviceName,
                operation: operation);

        // Then
        executeFailingOperation.Should()
            .Throw<ContentManagementServiceException>()
            .WithInnerException<Exception>()
            .Which.Should()
            .BeSameAs(expected: unclassifiedException);
    }
}