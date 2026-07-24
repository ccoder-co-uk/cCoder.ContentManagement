// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal interface IPageRenderService
{
    TResult Execute<TService, TResult>(
        string name,
        Func<TService, TResult> operation)
        where TService : notnull;
}