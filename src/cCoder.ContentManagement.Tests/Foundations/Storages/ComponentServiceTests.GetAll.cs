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
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ComponentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Component[] expectedItems = [CreateRandomComponent()];

        IQueryable<CmsDataModels.Component> components = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        componentBrokerMock.Setup(expression: x => x.GetAllComponents(ignoreFilters: false))
            .Returns(value: components);

        // When
        IQueryable<Component> result = componentService.GetAllComponent();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        componentBrokerMock.Verify(expression: x => x.GetAllComponents(ignoreFilters: false), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}