// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Rendering;

public partial class PageRenderServiceTests
{
    [Fact]
    public void ShouldExecuteNamedPageRenderServiceOperation()
    {
        // Given
        const string serviceName = "PageRender";
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
            pageRenderService.Execute<object, string>(
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
            pageRenderService.Execute<object, string>(
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
        const string serviceName = "MissingPageRenderService";
        InvalidOperationException dependencyException = new();
        Func<object, string> operation = _ => string.Empty;

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<object>(
                    name: serviceName))
            .Throws(exception: dependencyException);

        // When
        Action executeMissingService = () =>
            pageRenderService.Execute<object, string>(
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
        const string serviceName = "FailingPageRenderService";
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
            pageRenderService.Execute<object, string>(
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