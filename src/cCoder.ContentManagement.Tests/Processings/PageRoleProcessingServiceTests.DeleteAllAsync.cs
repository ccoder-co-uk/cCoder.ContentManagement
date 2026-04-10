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
using Xunit;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenItemUsesCompositeKeyForDeleteAllAsync()
    {
        // Given
        LocalPageRole link = new() { PageId = Random.Shared.Next(1, 1000), RoleId = Guid.NewGuid() };

        // When
        Func<Task> act = async () => await pageRoleProcessingService.DeleteAllAsync(new[] { link });

        // Then
        await act.Should().ThrowAsync<System.Security.SecurityException>();
    }

}















