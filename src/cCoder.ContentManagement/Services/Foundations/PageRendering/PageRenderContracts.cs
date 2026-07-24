// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Dependencies.Rendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheService
{
    PageCacheSlice GetPageRenderEngineRequestPageCacheSlice(PageRenderEngineRequest request);
}

internal interface IMarkupRenderService
{
    PageRenderResult RenderPageRenderSessionPageRenderResult(PageRenderSession session);
}