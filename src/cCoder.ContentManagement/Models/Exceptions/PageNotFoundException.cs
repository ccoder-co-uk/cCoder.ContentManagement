// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class PageNotFoundException(
    HttpPageRenderContext pageRenderContext)
        : Exception(message: "The requested page was not found.")
{
    public HttpPageRenderContext PageRenderContext { get; } =
        pageRenderContext;
}