// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface ICachedPageRenderService
{
    string MarkContentSecurityPolicyNonce(string markup);
}