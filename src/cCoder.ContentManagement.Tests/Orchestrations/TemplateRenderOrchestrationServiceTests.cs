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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateRenderOrchestrationServiceTests
{
    private readonly Mock<ITemplateRenderProcessingService> templateRenderProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly TemplateRenderOrchestrationService renderOrchestrationService;

    public TemplateRenderOrchestrationServiceTests()
    {
        templateRenderProcessingServiceMock = new Mock<ITemplateRenderProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new Mock<IAuthorizationProcessingService>(behavior: MockBehavior.Strict);
        renderOrchestrationService = new TemplateRenderOrchestrationService(
templateRenderProcessingService: templateRenderProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }
}