// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Authorizations;

internal interface IPageAuthorizationService
{
    ValueTask<HttpPageRenderContext> AuthorizeHttpPageRenderContextAsync(
        HttpPageRenderContext pageRenderContext);
}