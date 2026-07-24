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
    public async Task ShouldStampAuditFields_AndDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Resource resource = CreateRandomResource(id: 7);
        User currentUser = TestUsers.WithPrivilege(privilege: "resource_create");

        resourceServiceMock.Setup(expression: x => x.AddResourceAsync(newResource: resource))
            .ReturnsAsync(value: resource);

        // When
        Resource result = await resourceProcessingService.AddResourceAsync(newResource: resource);

        // Then
        Assert.Same(expected: resource, actual: result);
        Assert.Equal(expected: "test-user", actual: resource.CreatedBy);
        Assert.Equal(expected: "test-user", actual: resource.LastUpdatedBy);
        Assert.Equal(expected: resource.CreatedOn, actual: resource.LastUpdated);
        resourceServiceMock.Verify(expression: x => x.AddResourceAsync(newResource: resource), times: Times.Once);
    }

}