// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using FizzWare.NBuilder;
using Moq;
using LocalPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageCoordinationServiceTests
{
    private readonly Mock<IPageInfoOrchestrationService> pageInfoOrchestrationServiceMock;
    private readonly Mock<IContentOrchestrationService> contentOrchestrationServiceMock;
    private readonly Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock;
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock;
    private readonly PageCoordinationService coordinationService;
    private readonly PageStructureCoordinationService structureCoordinationService;

    public PageCoordinationServiceTests()
    {
        pageInfoOrchestrationServiceMock = new Mock<IPageInfoOrchestrationService>(
behavior: MockBehavior.Strict
        );

        contentOrchestrationServiceMock = new Mock<IContentOrchestrationService>(
behavior: MockBehavior.Strict
        );

        pageRoleOrchestrationServiceMock = new Mock<IPageRoleOrchestrationService>(
behavior: MockBehavior.Strict
        );

        pageOrchestrationServiceMock = new Mock<IPageOrchestrationService>(behavior: MockBehavior.Strict);

        coordinationService = new PageCoordinationService(
pageInfoOrchestrationService: pageInfoOrchestrationServiceMock.Object,
contentOrchestrationService: contentOrchestrationServiceMock.Object
        );

        structureCoordinationService = new PageStructureCoordinationService(
pageRoleOrchestrationService: pageRoleOrchestrationServiceMock.Object,
pageOrchestrationService: pageOrchestrationServiceMock.Object
        );
    }

    private static Page CreateRandomPage() =>
        Builder<Page>
            .CreateNew()
        .With(func: page => page.PageInfo = [Builder<PageInfo>.CreateNew()
        .Build()])
        .With(func: page => page.Contents = [Builder<Content>.CreateNew()
        .Build()])
        .With(func: page => page.Roles = [Builder<PageRole>.CreateNew()
        .Build()])
        .Build();

    private static LocalPageInfo[] ToLocalPageInfos(IEnumerable<PageInfo> pageInfos) =>
        [.. pageInfos];
}