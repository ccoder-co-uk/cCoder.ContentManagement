// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Extensions.Data;

internal static class CommonObjectQueryExtensions
{
    internal static CommonObject[] GetLatestCommonObjectsPaged(
        CoreDataContext coreDataContext,
        int pageSize)
    {
        int offset = 0;
        List<CommonObject> commonObjects = [];

        while (true)
        {
            CommonObject[] page = coreDataContext.CommonObjects
                .AsNoTracking()
                .GroupBy(keySelector: commonObject => new
                {
                    commonObject.Name,
                    commonObject.Culture,
                    commonObject.Key,
                    commonObject.Type
                })
                .Select(
                    selector: group => group
                        .OrderByDescending(keySelector: version => version.Version)
                        .First())
                .Skip(count: offset)
                .Take(count: pageSize)
                .ToArray();

            if (page.Length == 0)
            {
                break;
            }

            commonObjects.AddRange(collection: page);
            offset += pageSize;
        }

        return commonObjects.ToArray();
    }
}