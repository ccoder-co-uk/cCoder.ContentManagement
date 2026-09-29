// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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