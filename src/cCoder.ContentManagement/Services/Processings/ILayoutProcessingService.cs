// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ILayoutProcessingService
{
    Layout GetLayout(int layoutId);

    IQueryable<Layout> GetAllLayout(bool ignoreFilters = false);

    ValueTask<Layout> AddLayoutAsync(Layout newLayout);

    ValueTask<Layout> UpdateLayoutAsync(Layout updatedLayout);

    ValueTask DeleteAsync(int layoutId);

    ValueTask<IEnumerable<OperationResult<Layout>>> AddOrUpdateLayoutResult(IEnumerable<Layout> newLayout);

    ValueTask DeleteAllLayoutAsync(IEnumerable<Layout> deletedLayout);
}