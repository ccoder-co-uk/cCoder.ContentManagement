// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations.Caching;

namespace cCoder.ContentManagement.Exposures.Caching;

internal sealed class MetadataCacheManager(
    IMetadataCacheOrchestrationService orchestrationService)
        : IMetadataCache
{
    public string Get(string key, string culture) =>
        orchestrationService.Get(key: key, culture: culture);

    public string GetAll(string culture = "") =>
        orchestrationService.GetAll(culture: culture);

    public void Rebuild() =>
        orchestrationService.Rebuild();

    public string ToJson(string culture) =>
        orchestrationService.ToJson(culture: culture);
}