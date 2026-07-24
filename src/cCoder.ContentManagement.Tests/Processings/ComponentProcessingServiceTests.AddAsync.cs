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
using System.Security;



using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Component component = CreateRandomComponent();

        componentServiceMock.Setup(expression: x => x.AddComponentAsync(newComponent: component))
            .ReturnsAsync(value: component);

        // When
        Component result = await componentProcessingService.AddComponentAsync(newComponent: component);

        // Then
        Assert.Same(expected: component, actual: result);
        componentServiceMock.Verify(expression: x => x.AddComponentAsync(newComponent: component), times: Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Component component = CreateRandomComponent();

        componentServiceMock
            .Setup(expression: x => x.AddComponentAsync(newComponent: component))
            .ThrowsAsync(exception: new SecurityException(message: "Access Denied!"));

        // When

        await Assert.ThrowsAsync<cCoder.ContentManagement.Models.Exceptions.ContentManagementSecurityException>(testCode: async () =>
            await componentProcessingService.AddComponentAsync(newComponent: component)
        );

        // Then
    }

}