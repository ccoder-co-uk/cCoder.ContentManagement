// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheService
{
    PageCacheSlice GetPageCacheSlice();
}

internal interface IMarkupRenderService
{
    PageRenderSession RenderPageRenderSession(PageRenderSession session);
}