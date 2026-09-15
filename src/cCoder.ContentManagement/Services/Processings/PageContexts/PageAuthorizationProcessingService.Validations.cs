// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings.PageContexts;

internal sealed partial class PageAuthorizationProcessingService
{
    private static void ValidateAuthorizeHttpPageRenderContextAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}