// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IContentOrchestrationService
{
    Content GetContent(int contentId);

    IQueryable<Content> GetAllContent(bool ignoreFilters = false);

    ValueTask<Content> AddContentAsync(Content newContent);

    ValueTask<Content> UpdateContentAsync(Content updatedContent);

    ValueTask DeleteAsync(int contentId);

    ValueTask<IEnumerable<Result<Content>>> AddOrUpdateContentResult(IEnumerable<Content> newContent);

    ValueTask DeleteAllContentAsync(IEnumerable<Content> deletedContent);
}