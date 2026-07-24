// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class CurrentAppManager(
    ICurrentAppProcessingService currentAppProcessingService)
    : ICurrentAppResolver
{
    public App ResolveCurrentApp() =>
        currentAppProcessingService.ResolveCurrentApp();
}