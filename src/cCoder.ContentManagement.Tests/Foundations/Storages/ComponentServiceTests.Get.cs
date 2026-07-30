// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Component component = CreateRandomComponent(id: 7);

        componentBrokerMock.Setup(expression: x => x.GetAllComponents())
            .Returns(value: new[] { component }.AsQueryable());

        // When
        Component result = componentService.GetComponent(componentId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: component);

        componentBrokerMock.Verify(expression: x => x.GetAllComponents(), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationManagerMock.VerifyNoOtherCalls();
    }

}