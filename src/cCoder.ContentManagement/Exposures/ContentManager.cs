// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ContentManager(IContentOrchestrationService service) : IContentManager
{
    public Content GetContent(int contentId) =>
        service.GetContent(contentId: contentId);

    public IQueryable<Content> GetAllContent(bool ignoreFilters = false) =>
        service.GetAllContent(ignoreFilters: ignoreFilters);

    public ValueTask<Content> AddContentAsync(Content newContent) =>
        service.AddContentAsync(newContent: newContent);

    public ValueTask<Content> UpdateContentAsync(Content updatedContent) =>
        service.UpdateContentAsync(updatedContent: updatedContent);

    public ValueTask DeleteAsync(int contentId) =>
        service.DeleteAsync(contentId: contentId);

    public ValueTask<IEnumerable<OperationResult<Content>>> AddOrUpdateContentResult(
        IEnumerable<Content> newContent) =>
        service.AddOrUpdateContentResult(newContent: newContent);

    public ValueTask DeleteAllContentAsync(IEnumerable<Content> deletedContent) =>
        service.DeleteAllContentAsync(deletedContent: deletedContent);
}