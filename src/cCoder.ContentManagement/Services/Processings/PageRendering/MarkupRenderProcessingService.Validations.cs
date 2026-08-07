// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MarkupRenderProcessingService
{
    private static void ValidateRenderRenderSession(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderSession(RenderSession session)
    {
        if (session.Request == null)
        {
            throw new ArgumentException(
                message: "RenderSession.Request is required.");
        }

        if (session.Target == null)
        {
            throw new ArgumentException(
                message: "RenderSession.Target is required.");
        }

        if (session.Target.Scope == RenderScope.Page
            && (session.Page == null || session.Layout == null))
        {
            throw new ArgumentException(
                message: "Page rendering requires Page and Layout context.");
        }
    }
}