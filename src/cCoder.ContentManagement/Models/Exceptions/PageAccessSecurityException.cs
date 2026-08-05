// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class PageAccessSecurityException(
    HttpPageRenderContext pageRenderContext)
        : SecurityException(message: "The current user cannot access the requested page.")
{
    public HttpPageRenderContext PageRenderContext { get; } =
        pageRenderContext;
}