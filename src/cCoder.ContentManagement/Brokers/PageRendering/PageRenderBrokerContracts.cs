
using cCoder.ContentManagement.Rendering.Models;
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
    string GetMetadata(string name, string culture);
}

internal interface ICommonObjectReaderBroker
{
    IReadOnlyDictionary<string, PageRenderResource> GetResourcesByLookup();

    IReadOnlyDictionary<string, PageRenderComponent> GetComponentsByName();

    IReadOnlyDictionary<string, PageRenderScript> GetScriptsByName();
}
