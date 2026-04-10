using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Exposures.Controllers;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using App = cCoder.Data.Models.CMS.App;
using Culture = cCoder.Data.Models.CMS.Culture;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Tests.Exposures;

public class ControllerGetAllTests
{
    [Fact]
    public void AppGetAll_ShouldReturnServiceQueryableUntouched()
    {
        Mock<IAppOrchestrationService> serviceMock = new();
        IQueryable<App> expectedApps = new[] { new App { Id = 1, Name = "App" } }.AsQueryable();

        serviceMock.Setup(service => service.GetAll(false)).Returns(expectedApps);

        AppController controller = new(
            serviceMock.Object,
            Mock.Of<IAuthorizationBroker>(),
            Mock.Of<ILogger<AppController>>());

        OkObjectResult result = controller.GetAll(null!).Should().BeOfType<OkObjectResult>().Subject;

        result.Value.Should().BeSameAs(expectedApps);
    }

    [Fact]
    public void CultureGetAll_ShouldReturnServiceQueryableUntouched()
    {
        Mock<ICultureOrchestrationService> serviceMock = new();
        IQueryable<Culture> expectedCultures = new[] { new Culture { Id = "en-GB", Name = "English" } }.AsQueryable();

        serviceMock.Setup(service => service.GetAll(false)).Returns(expectedCultures);

        CultureController controller = new(
            serviceMock.Object,
            Mock.Of<ILogger<CultureController>>());

        OkObjectResult result = controller.GetAll(null!).Should().BeOfType<OkObjectResult>().Subject;

        result.Value.Should().BeSameAs(expectedCultures);
    }

    [Fact]
    public void PageGetAll_ShouldReturnServiceQueryableUntouched()
    {
        Mock<IPageOrchestrationService> serviceMock = new();
        IQueryable<Page> expectedPages = new[] { new Page { Id = 1, AppId = 1, Name = "Admin", Path = "Admin" } }.AsQueryable();

        serviceMock.Setup(service => service.GetAll(false)).Returns(expectedPages);

        PageController controller = new(
            serviceMock.Object,
            Mock.Of<IPageRenderCoordinationService>(),
            Mock.Of<ILogger<PageController>>());

        OkObjectResult result = controller.Get(null!).Should().BeOfType<OkObjectResult>().Subject;

        result.Value.Should().BeSameAs(expectedPages);
    }

    [Fact]
    public void SubmissionGetAll_ShouldReturnServiceQueryableUntouched()
    {
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

        serviceMock.Setup(service => service.GetAll(false)).Returns(expectedSubmissions);

        SubmissionController controller = new(
            serviceMock.Object,
            Mock.Of<ILogger<SubmissionController>>());

        OkObjectResult result = controller.GetAll(null!).Should().BeOfType<OkObjectResult>().Subject;

        result.Value.Should().BeSameAs(expectedSubmissions);
    }
}
