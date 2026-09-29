// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings.PageContexts;

internal interface IPageAuthorizationProcessingService
{
    ValueTask<HttpPageRenderContext> AuthorizeHttpPageRenderContextAsync(
        HttpPageRenderContext httpPageRenderContext);
}