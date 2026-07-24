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
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public async Task ShouldStampLastUpdatedFields_AndDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Resource resource = CreateRandomResource(id: 7);
        User currentUser = TestUsers.WithPrivilege(privilege: "resource_update");

        resourceServiceMock.Setup(expression: x => x.UpdateResourceAsync(updatedResource: resource))
            .ReturnsAsync(value: resource);

        // When
        Resource result = await resourceProcessingService.UpdateResourceAsync(updatedResource: resource);

        // Then
        Assert.Same(expected: resource, actual: result);
        Assert.Equal(expected: "test-user", actual: resource.LastUpdatedBy);
        resourceServiceMock.Verify(expression: x => x.UpdateResourceAsync(updatedResource: resource), times: Times.Once);
    }

}