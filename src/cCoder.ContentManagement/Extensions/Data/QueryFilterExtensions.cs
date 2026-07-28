// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Extensions.Data;

internal static class QueryFilterExtensions
{
    internal static IQueryable<T> Apply<T>(
        IQueryable<T> query,
        bool ignoreFilters)
        where T : class =>
        ignoreFilters
            ? query.IgnoreQueryFilters()
            : query;
}