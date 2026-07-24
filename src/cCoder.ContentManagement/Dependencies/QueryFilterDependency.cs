// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Dependencies;

internal static class QueryFilterDependency
{
    internal static IQueryable<T> Apply<T>(
        IQueryable<T> query,
        bool ignoreFilters)
        where T : class =>
        ignoreFilters
            ? query.IgnoreQueryFilters()
            : query;
}
