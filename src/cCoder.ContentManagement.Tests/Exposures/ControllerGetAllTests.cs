// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Controllers;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using App = cCoder.Data.Models.CMS.App;
using Culture = cCoder.Data.Models.CMS.Culture;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Tests.Exposures;

public partial class ControllerGetAllTests
{
    [Fact]
    public void AppGetAll_ShouldReturnServiceQueryableUntouched()
    {
        // Given
        Mock<IAppManager> managerMock = new();
        IQueryable<App> expectedApps = new[] { new App { Id = 1, Name = "App" } }.AsQueryable();

        managerMock.Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Returns(value: expectedApps);

        // When
        AppController controller = new(
            manager: managerMock.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // Then
        OkObjectResult result = controller.GetAll(queryOptions: null!)
            .Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        result.Value.Should()
            .BeSameAs(expected: expectedApps);
    }

    [Fact]
    public void CultureGetAll_ShouldReturnServiceQueryableUntouched()
    {
        // Given
        Mock<ICultureOrchestrationService> serviceMock = new();
        IQueryable<Culture> expectedCultures = new[] { new Culture { Id = "en-GB", Name = "English" } }.AsQueryable();

        serviceMock.Setup(expression: service => service.GetAllCulture())
            .Returns(value: expectedCultures);

        // When
        CultureController controller = new(
            service: serviceMock.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // Then
        OkObjectResult result = controller.GetAll(queryOptions: null!)
            .Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        result.Value.Should()
            .BeSameAs(expected: expectedCultures);
    }

    [Fact]
    public void PageGetAll_ShouldReturnServiceQueryableUntouched()
    {
        // Given
        Mock<IPageManager> managerMock = new();
        IQueryable<Page> expectedPages = new[] { new Page { Id = 1, AppId = 1, Name = "Admin", Path = "Admin" } }.AsQueryable();

        managerMock.Setup(expression: manager => manager.GetAll())
            .Returns(value: expectedPages);

        // When
        PageController controller = new(
            manager: managerMock.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // Then
        OkObjectResult result = controller.Get(queryOptions: null!)
            .Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        result.Value.Should()
            .BeSameAs(expected: expectedPages);
    }

    [Fact]
    public void SubmissionGetAll_ShouldReturnServiceQueryableUntouched()
    {
        // Given
        Mock<ISubmissionOrchestrationService> serviceMock = new();

        IQueryable<Submission> expectedSubmissions = new[]
        {
            new Submission
            {
                Id = Guid.NewGuid(),
                AppId = 1,
                SourceComponent = "Acceptance",
                State = "New"
            }
        }.AsQueryable();

        serviceMock.Setup(expression: service => service.GetAllSubmission())
            .Returns(value: expectedSubmissions);

        // When
        SubmissionController controller = new(
            service: serviceMock.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // Then
        OkObjectResult result = controller.GetAll(queryOptions: null!)
            .Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        result.Value.Should()
            .BeSameAs(expected: expectedSubmissions);
    }
}