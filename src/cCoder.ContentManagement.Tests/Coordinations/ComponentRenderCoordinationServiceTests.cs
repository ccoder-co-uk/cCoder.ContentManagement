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

public partial class ComponentRenderCoordinationServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IComponentRenderOrchestrationService> orchestrationServiceMock = new();
    private readonly ComponentRenderCoordinationService coordinationService;

    public ComponentRenderCoordinationServiceTests()
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

        coordinationService = new ComponentRenderCoordinationService(
authorizationBroker: authorizationBrokerMock.Object,
componentRenderOrchestrationService: orchestrationServiceMock.Object);
    }

    [Fact]
    public void ShouldDefaultCultureFromCurrentUser()
    {
        // Given
        orchestrationServiceMock
            .Setup(expression: x => x.RenderUser(appId: 1, name: "Hero", user: It.Is<User>(match: user => user.Id == "test-user"), culture: "en-GB", theme: "Default"))
            .Returns(value: "<section>hero</section>");

        // When
        string result = coordinationService.Render(appId: 1, name: "Hero", culture: null, theme: "Default");

        // Then
        result.Should()
            .Be(expected: "<section>hero</section>");

        orchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenThemeIsMissing()
    {
        // Given
        // When
        Action act = () => coordinationService.Render(appId: 1, name: "Hero", culture: "en-GB", theme: null);

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "theme is required.");
    }
}