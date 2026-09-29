// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IContentProcessingService
{
    Content GetContent(int contentId);

    IQueryable<Content> GetAllContent(bool ignoreFilters = false);

    int? GetAppIdByPageId(int pageId);

    ValueTask<Content> AddContentAsync(Content newContent);

    ValueTask<Content> UpdateContentAsync(Content updatedContent);

    ValueTask DeleteAsync(int contentId);

    ValueTask<IEnumerable<OperationResult<Content>>> AddOrUpdateContentResult(IEnumerable<Content> newContent);

    ValueTask DeleteAllContentAsync(IEnumerable<Content> deletedContent);
}