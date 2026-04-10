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

public class TemplateRenderCoordinationServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<ITemplateRenderOrchestrationService> orchestrationServiceMock = new();
    private readonly TemplateRenderCoordinationService coordinationService;

    public TemplateRenderCoordinationServiceTests()
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

        coordinationService = new TemplateRenderCoordinationService(
            authorizationBrokerMock.Object,
            orchestrationServiceMock.Object);
    }

    [Fact]
    public void ShouldDefaultCultureFromCurrentUser()
    {
        object model = new { Name = "Ward" };

        orchestrationServiceMock
            .Setup(x => x.Render(1, "Welcome", "en-GB", model, It.Is<User>(user => user.Id == "test-user")))
            .Returns("<main>welcome</main>");

        string result = coordinationService.Render(1, "Welcome", null, model);

        result.Should().Be("<main>welcome</main>");
        orchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenModelIsMissing()
    {
        Action act = () => coordinationService.Render(1, "Welcome", "en-GB", null);

        act.Should().Throw<ValidationException>().WithMessage("model is required.");
    }
}




