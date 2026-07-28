// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures.Caching;

public interface IMetadataCache
{
    string Get(string key, string culture);

    string GetAll(string culture = "");

    void Rebuild();

    string ToJson(string culture);
}