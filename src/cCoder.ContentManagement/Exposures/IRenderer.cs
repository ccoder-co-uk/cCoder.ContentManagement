// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Exposures;

public interface IRenderer
{
    ValueTask<RenderResult> RenderTemplateRenderResultAsync(
        string name,
        object model);

    ValueTask<RenderResult> RenderComponentRenderResultAsync(string name);
}