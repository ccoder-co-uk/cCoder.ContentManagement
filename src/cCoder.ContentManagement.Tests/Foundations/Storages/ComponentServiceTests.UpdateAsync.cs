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
    public async Task ShouldDelegateToBrokerWhenUpdateAsync()
    {
        // Given
        Component component = CreateRandomComponent(id: 7, appId: 7);

        CmsDataModels.Component submitted = null;


        componentBrokerMock
            .Setup(expression: x => x.UpdateComponentAsync(updatedComponent: It.IsAny<CmsDataModels.Component>()))
            .Callback<CmsDataModels.Component>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Component value) => value);

        // When
        Component result = await componentService.UpdateComponentAsync(updatedComponent: component);

        // Then

        result.Should()
            .BeSameAs(expected: component);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: component);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: component,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
            );

        result
            .Should()
            .BeEquivalentTo(
expectation: component,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "CreatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdated")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "LastUpdatedOn")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "UpdatedBy")
                        )
            .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith(value: "Created")
                        )
            );

        componentBrokerMock.Verify(expression: x => x.UpdateComponentAsync(updatedComponent: It.IsAny<CmsDataModels.Component>()), times: Times.Once);
        componentBrokerMock.VerifyNoOtherCalls();
    }

}