// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class CurrentAppManager(
    ICurrentAppOrchestrationService currentAppOrchestrationService)
    : ICurrentAppResolver
{
    public App ResolveCurrentApp() =>
        currentAppOrchestrationService.ResolveCurrentApp();
}