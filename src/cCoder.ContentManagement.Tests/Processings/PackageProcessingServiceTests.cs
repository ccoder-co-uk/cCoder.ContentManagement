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
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;




namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PackageProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IPackageService> packageServiceMock = new();
    private readonly PackageProcessingService packageProcessingService;

    public PackageProcessingServiceTests()
    {
        packageProcessingService = new PackageProcessingService(
            service: packageServiceMock.Object);
    }

    private static Package CreateRandomPackage() =>
        Builder<Package>
            .CreateNew()
        .With(func: x => x.Id = Guid.NewGuid())
        .With(func: x => x.Name = $"Package-{Guid.NewGuid():N}")
        .With(func: x => x.Description = "Description")
        .With(func: x => x.Category = "Category")
        .With(func: x => x.SourceApi = "https://example.test/api")
        .With(func: x => x.Items = [])
        .Build();

    private static PackageItem CreateRandomPackageItem() =>
        Builder<PackageItem>
            .CreateNew()
        .With(func: x => x.Type = $"Type-{Guid.NewGuid():N}")
        .With(func: x => x.Data = $"Data-{Guid.NewGuid():N}")
        .With(func: x => x.Package = null)
        .Build();
}