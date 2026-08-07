// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.OData;
using cCoder.ContentManagement.Models.OData;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers.OData;

public sealed partial class ContentManagementModelBrokerTests
{
    [Fact]
    public void ShouldBuildContentManagementEdmModel()
    {
        // Given
        ContentManagementModelBroker broker = new();

        // When
        ODataModel model = broker.Build();

        // Then
        model.EDMModel
            .Should()
            .NotBeNull();

        model.EDMModel.SchemaElements
            .OfType<IEdmComplexType>()
            .Select(selector: complexType => complexType.Name)
            .Should()
            .Contain(
                expected:
                [
                    "RenderResult",
                    "PageRenderResult",
                    "TemplateRenderResult",
                    "ComponentRenderResult"
                ]);
    }
}