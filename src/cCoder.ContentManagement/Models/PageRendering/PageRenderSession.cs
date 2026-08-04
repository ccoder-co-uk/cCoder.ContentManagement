// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Models.PageRendering;

internal sealed class PageRenderSession
{
    public PageRenderEngineRequest Request { get; init; }

    public ContentManagementConfiguration Config { get; set; }

    public PageRenderApp App { get; set; }

    public PageRenderPage Page { get; set; }

    public PageRenderUser User { get; set; }

    public PageRenderLayout Layout { get; set; }

    public IReadOnlyList<PageRenderResource> Resources { get; set; }
    public IReadOnlyDictionary<string, PageRenderResource> ResourcesByLookup { get; set; }
    public IDictionary<string, PageRenderComponent> ComponentsByName { get; set; }
    public IDictionary<string, PageRenderScript> ScriptsByName { get; set; }
    public Func<string, string> MetadataResolver { get; set; }
    public IReadOnlyDictionary<string, PageRenderResource> CommonResourcesByLookup { get; set; }
    public IReadOnlyDictionary<string, PageRenderComponent> CommonComponentsByName { get; set; }
    public IReadOnlyDictionary<string, PageRenderScript> CommonScriptsByName { get; set; }

    public PageRenderResult Result { get; set; }

}