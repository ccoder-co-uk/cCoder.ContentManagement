// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal interface IComponentReaderBroker
{
    IEnumerable<Component> GetComponents(int appId);

    Component GetComponent(int appId, string name);
}

internal interface IScriptReaderBroker
{
    IEnumerable<Script> GetScripts(int appId);

    Script GetScript(int appId, string name);
}

internal interface IMetadataReaderBroker
{
    string Get(string key, string culture);

    string GetMetadata(string name, string culture);
}

internal interface ICommonObjectReaderBroker
{
    T[] GetAll<T>();

    T Get<T>(string key);

    void Set(string key, object item);

    IEnumerable<CommonObject> GetLatestSet();

    IReadOnlyDictionary<string, PageRenderResource> GetResourcesByLookup();

    IReadOnlyDictionary<string, PageRenderComponent> GetComponentsByName();

    IReadOnlyDictionary<string, PageRenderScript> GetScriptsByName();

    IReadOnlyDictionary<string, PageRenderStyle> GetStylesByName();
}