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



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateNewVersionAndAddItWhenUserHasPrivilegesForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        User actor = TestUsers.WithPrivileges(
            new[] { "commonobject_create", "commonobject_update" }
        );
        CommonObject commonObject = CreateRandomCommonObject(
            "Core/Other"
        );
        CommonObject existingVersion = CreateRandomCommonObject(
            "Core/Other"
        );
        existingVersion.Name = commonObject.Name;
        existingVersion.Type = commonObject.Type;
        existingVersion.Culture = commonObject.Culture;
        existingVersion.Key = commonObject.Key;
        existingVersion.Version = 2;

        currentUser = actor;

        commonObjectServiceMock
            .Setup(x => x.GetAll())
            .Returns(new[] { existingVersion }.AsQueryable());

        commonObjectServiceMock
            .Setup(x => x.AddAsync(It.IsAny<CommonObject>()))
            .ReturnsAsync((CommonObject item) => item);

        // When
        CommonObject result =
            await commonObjectProcessingService.UpdateAsync(commonObject);

        // Then
        result.Id.Should().Be(0);
        result.Version.Should().Be(3);
        result.CreatedBy.Should().Be(actor.Id);
        result.LastUpdatedBy.Should().Be(actor.Id);
        commonObjectServiceMock.Verify(x => x.GetAll(), Times.Exactly(2));
        commonObjectServiceMock.Verify(
            x =>
                x.AddAsync(
                    It.Is<CommonObject>(item =>
                        item.Id == 0 && item.Version == 3
                    )
                ),
            Times.Once
        );
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}






















