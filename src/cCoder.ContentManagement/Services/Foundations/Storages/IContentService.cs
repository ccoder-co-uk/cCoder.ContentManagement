// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IContentService
{
    Content GetContent(int contentId, bool ignoreFilters = false);

    IQueryable<Content> GetAllContent(bool ignoreFilters = false);

    ValueTask<Content> AddContentAsync(Content newContent);

    ValueTask<Content> UpdateContentAsync(Content updatedContent);

    ValueTask DeleteAsync(int contentId);
}