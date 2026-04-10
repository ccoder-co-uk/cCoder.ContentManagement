using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using System.ComponentModel.DataAnnotations;



using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;

namespace cCoder.ContentManagement.Tests.Coordinations;

public class ComponentRenderCoordinationServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IComponentRenderOrchestrationService> orchestrationServiceMock = new();
    private readonly ComponentRenderCoordinationService coordinationService;

    public ComponentRenderCoordinationServiceTests()
    {
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new DataUser
        {
            Id = "test-user",
            DefaultCultureId = "en-GB",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = []
        });

        coordinationService = new ComponentRenderCoordinationService(
            authorizationBrokerMock.Object,
            orchestrationServiceMock.Object);
    }

    [Fact]
    public void ShouldDefaultCultureFromCurrentUser()
    {
        orchestrationServiceMock
            .Setup(x => x.Render(1, "Hero", It.Is<User>(user => user.Id == "test-user"), "en-GB", "Default"))
            .Returns("<section>hero</section>");

        string result = coordinationService.Render(1, "Hero", null, "Default");

        result.Should().Be("<section>hero</section>");
        orchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenThemeIsMissing()
    {
        Action act = () => coordinationService.Render(1, "Hero", "en-GB", null);

        act.Should().Throw<ValidationException>().WithMessage("theme is required.");
    }
}




