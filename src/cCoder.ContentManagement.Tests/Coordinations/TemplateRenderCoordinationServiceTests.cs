// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class TemplateRenderCoordinationServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<ITemplateRenderOrchestrationService> orchestrationServiceMock = new();
    private readonly TemplateRenderCoordinationService coordinationService;

    public TemplateRenderCoordinationServiceTests()
    {
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new DataUser
            {
                Id = "test-user",
                DefaultCultureId = "en-GB",
                DisplayName = "Test User",
                Email = "test@example.com",
                IsActive = true,
                Roles = []
            });

        coordinationService = new TemplateRenderCoordinationService(
authorizationBroker: authorizationBrokerMock.Object,
templateRenderOrchestrationService: orchestrationServiceMock.Object);
    }

    [Fact]
    public void ShouldDefaultCultureFromCurrentUser()
    {
        // Given
        object model = new { Name = "Ward" };

        orchestrationServiceMock
            .Setup(expression: x => x.RenderUser(appId: 1, name: "Welcome", culture: "en-GB", model: model, user: It.Is<User>(match: user => user.Id == "test-user")))
            .Returns(value: "<main>welcome</main>");

        // When
        string result = coordinationService.Render(appId: 1, name: "Welcome", culture: null, model: model);

        // Then
        result.Should()
            .Be(expected: "<main>welcome</main>");

        orchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenModelIsMissing()
    {
        // Given
        // When
        Action act = () => coordinationService.Render(appId: 1, name: "Welcome", culture: "en-GB", model: null);

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "model is required.");
    }
}