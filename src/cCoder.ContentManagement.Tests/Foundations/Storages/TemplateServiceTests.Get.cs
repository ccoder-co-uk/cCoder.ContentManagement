// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class TemplateServiceTests
{
    [Fact]
    public void ShouldReturnTemplateWhenGet()
    {
        // Given
        Template template = CreateRandomTemplate(id: 5);

        templateBrokerMock.Setup(expression: x => x.GetAllTemplates())
            .Returns(value: new[] { template }.AsQueryable());

        // When
        Template result = templateService.GetTemplate(templateId: 5);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: template);

        templateBrokerMock.Verify(expression: x => x.GetAllTemplates(), times: Times.Once);
        templateBrokerMock.VerifyNoOtherCalls();
    }

}