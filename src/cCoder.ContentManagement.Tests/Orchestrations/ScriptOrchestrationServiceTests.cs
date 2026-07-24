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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class ScriptOrchestrationServiceTests
{
    private readonly Mock<IScriptProcessingService> scriptProcessingServiceMock;
    private readonly Mock<IScriptEventProcessingService> scriptEventProcessingServiceMock;
    private readonly ScriptOrchestrationService orchestrationService;

    public ScriptOrchestrationServiceTests()
    {
        scriptProcessingServiceMock = new Mock<IScriptProcessingService>(behavior: MockBehavior.Strict);
        scriptEventProcessingServiceMock = new Mock<IScriptEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new ScriptOrchestrationService(
processingService: scriptProcessingServiceMock.Object,
eventService: scriptEventProcessingServiceMock.Object
        );
    }

    private static Script CreateRandomScript() =>
        Builder<Script>.CreateNew()
        .Build();
}