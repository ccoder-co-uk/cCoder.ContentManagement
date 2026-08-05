// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Coordinations;

internal interface IPageRenderCoordinationService
{
    ValueTask<PageRenderResponse> RenderPageRenderResponseAsync();
}