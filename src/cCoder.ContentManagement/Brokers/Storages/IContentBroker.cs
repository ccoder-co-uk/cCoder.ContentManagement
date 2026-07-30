// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IContentBroker
{
    IQueryable<Content> GetAllContents();

    IQueryable<Content> GetAllContentsIgnoringFilters();

    ValueTask<Content> AddContentAsync(Content newContent);

    ValueTask<Content> UpdateContentAsync(Content updatedContent);

    ValueTask<int> DeleteContentAsync(Content deletedContent);

    ValueTask DeleteAllContentsAsync(IEnumerable<Content> deletedContent);
}