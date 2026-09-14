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
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenDeleteAsync()
    {
        // Given
        Component component = CreateRandomComponent(id: 9, appId: 7);

        componentBrokerMock.Setup(expression: x => x.GetAllComponents())
            .Returns(value: new[] { component }.AsQueryable());

        componentBrokerMock.Setup(expression: x => x.DeleteComponentAsync(deletedComponent: It.IsAny<CmsDataModels.Component>()))
            .ReturnsAsync(value: 1);

        // When
        await componentService.DeleteAsync(componentId: 9);

        // Then
        componentBrokerMock.Verify(expression: x => x.GetAllComponents(), times: Times.Once);
        componentBrokerMock.Verify(expression: x => x.DeleteComponentAsync(deletedComponent: It.Is<CmsDataModels.Component>(match: actual => actual.Id == component.Id)), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
    }

}