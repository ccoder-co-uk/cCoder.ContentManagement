// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class PageCoordinationService(
    IPageInfoOrchestrationService pageInfoOrchestrationService,
    IContentOrchestrationService contentOrchestrationService)
    : IPageCoordinationService
{
    public ValueTask HandlePageAddAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageAddAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        if (page.PageInfo != null)
        {
            PageInfo[] pageInfos = page.PageInfo.Select(selector: (PageInfo pageInfo) => new PageInfo
            {
                Id = pageInfo.Id,
                PageId = page.Id,
                CultureId = pageInfo.CultureId,
                Title = pageInfo.Title,
                Description = pageInfo.Description,
                Keywords = pageInfo.Keywords
            })
                .ToArray();

            foreach (PageInfo pageInfo in pageInfos)
            {
                if (pageInfo.Id < 1)
                {
                    await pageInfoOrchestrationService.AddPageInfoAsync(newPageInfo: pageInfo);
                }
                else
                {
                    await pageInfoOrchestrationService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);
                }
            }
        }

        if (page.Contents != null)
        {
            Content[] contents = page.Contents
                .Select(selector: content =>
                {
                    content.PageId = page.Id;
                    return content;
                })
                .ToArray();

            foreach (Content content in contents)
            {
                if (content.Id < 1)
                {
                    await contentOrchestrationService.AddContentAsync(newContent: content);
                }
                else
                {
                    await contentOrchestrationService.UpdateContentAsync(updatedContent: content);
                }
            }
        }

    }, isValueTask: true);

    public ValueTask HandlePageUpdateAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageUpdateAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        if (page.PageInfo != null)
        {
            PageInfo[] existingPageInfos = pageInfoOrchestrationService.GetAllPageInfo(ignoreFilters: true)
                .Where(predicate: pageInfo => pageInfo.PageId == page.Id)
                .ToArray();

            await SyncPageInfoAsync(pageId: page.Id, existingItems: existingPageInfos, incomingItems: page.PageInfo);
        }

        if (page.Contents != null)
        {
            Content[] existingContents = contentOrchestrationService.GetAllContent(ignoreFilters: true)
                .Where(predicate: content => content.PageId == page.Id)
                .ToArray();

            await SyncContentsAsync(pageId: page.Id, existingItems: existingContents, incomingItems: page.Contents);
        }

    }, isValueTask: true);

    public ValueTask HandlePageDeleteAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageDeleteAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        IEnumerable<PageInfo> pageInfosToDelete = pageInfoOrchestrationService.GetAllPageInfo(ignoreFilters: true)
            .Where(predicate: pageInfo => pageInfo.PageId == page.Id)
            .ToArray();

        IEnumerable<Content> contentsToDelete = contentOrchestrationService.GetAllContent(ignoreFilters: true)
            .Where(predicate: content => content.PageId == page.Id)
            .ToArray();

        await pageInfoOrchestrationService.DeleteAllPageInfoAsync(deletedPageInfo: pageInfosToDelete);
        await contentOrchestrationService.DeleteAllContentAsync(deletedContent: contentsToDelete);

    }, isValueTask: true);

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return page;
    }

    private async ValueTask SyncPageInfoAsync(
        int pageId,
        IEnumerable<PageInfo> existingItems,
        IEnumerable<PageInfo> incomingItems)
    {
        PageInfo[] existingArray = existingItems.ToArray();
        PageInfo[] incomingArray = incomingItems.ToArray();

        foreach (PageInfo incoming in incomingArray)
        {
            PageInfo existing = existingArray.FirstOrDefault(predicate: item =>
                string.Equals(a: item.CultureId, b: incoming.CultureId, comparisonType: StringComparison.Ordinal));

            if (existing == null)
            {
                await pageInfoOrchestrationService.AddPageInfoAsync(newPageInfo: new PageInfo
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Title = incoming.Title,
                    Description = incoming.Description,
                    Keywords = incoming.Keywords
                });

                continue;
            }

            await pageInfoOrchestrationService.UpdatePageInfoAsync(updatedPageInfo: new PageInfo
            {
                Id = existing.Id,
                PageId = pageId,
                CultureId = incoming.CultureId,
                Title = incoming.Title,
                Description = incoming.Description,
                Keywords = incoming.Keywords
            });
        }

        foreach (PageInfo existing in existingArray
            .Where(predicate: item => item.CultureId != string.Empty && !incomingArray.Any(predicate: incoming =>
                string.Equals(a: incoming.CultureId, b: item.CultureId, comparisonType: StringComparison.Ordinal))))
        {
            await pageInfoOrchestrationService.DeleteAsync(pageInfoId: existing.Id);
        }
    }

    private async ValueTask SyncContentsAsync(
        int pageId,
        IEnumerable<Content> existingItems,
        IEnumerable<Content> incomingItems)
    {
        Content[] existingArray = existingItems.ToArray();
        Content[] incomingArray = incomingItems.ToArray();

        foreach (Content incoming in incomingArray)
        {
            Content existing = existingArray.FirstOrDefault(predicate: item =>
                string.Equals(a: item.Name, b: incoming.Name, comparisonType: StringComparison.Ordinal) &&
                string.Equals(a: item.CultureId, b: incoming.CultureId, comparisonType: StringComparison.Ordinal));

            if (existing == null)
            {
                await contentOrchestrationService.AddContentAsync(newContent: new Content
                {
                    PageId = pageId,
                    CultureId = incoming.CultureId,
                    Name = incoming.Name,
                    Html = incoming.Html
                });

                continue;
            }

            await contentOrchestrationService.UpdateContentAsync(updatedContent: new Content
            {
                Id = existing.Id,
                PageId = pageId,
                CultureId = incoming.CultureId,
                Name = incoming.Name,
                Html = incoming.Html
            });
        }

        foreach (Content existing in existingArray
            .Where(predicate: item => item.CultureId != string.Empty && !incomingArray.Any(predicate: incoming =>
                string.Equals(a: incoming.Name, b: item.Name, comparisonType: StringComparison.Ordinal) &&
                string.Equals(a: incoming.CultureId, b: item.CultureId, comparisonType: StringComparison.Ordinal))))
        {
            await contentOrchestrationService.DeleteAsync(contentId: existing.Id);
        }
    }

}