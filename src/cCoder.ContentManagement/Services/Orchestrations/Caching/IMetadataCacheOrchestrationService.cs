// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal interface IMetadataCacheOrchestrationService
{
    string Get(string key, string culture);

    string GetAll(string culture);

    void Rebuild();

    string ToJson(string culture);
}