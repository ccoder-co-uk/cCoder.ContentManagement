// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class PageImportOrchestrationService(
    IPageInfoProcessingService pageInfoProcessingService,
    IContentProcessingService contentProcessingService)
    : IPageImportOrchestrationService
{
    public ValueTask HandlePageImportAsync(Page page) =>
        TryCatch(operation: async () =>
    {
        ValidateHandlePageImportAsync(inputs: [page]);
        ValidatePage(page: page, parameterName: "page");

        if (page.PageInfo != null && page.PageInfo.Any())
        {
            PageInfo[] existingItems = pageInfoProcessingService
                .GetAllPageInfo(ignoreFilters: true)
                .Where(predicate: item => item.PageId == page.Id)
                .ToArray();

            foreach (PageInfo incoming in page.PageInfo)
            {
                PageInfo existing = existingItems.FirstOrDefault(predicate: item =>
                    item.CultureId == incoming.CultureId);

                incoming.Id = existing?.Id ?? 0;
                incoming.PageId = page.Id;

                _ = incoming.Id < 1
                    ? await pageInfoProcessingService.AddPageInfoAsync(newPageInfo: incoming)
                    : await pageInfoProcessingService.UpdatePageInfoAsync(updatedPageInfo: incoming);
            }
        }

        if (page.Contents != null && page.Contents.Any())
        {
            Content[] existingItems = contentProcessingService
                .GetAllContent(ignoreFilters: true)
                .Where(predicate: item => item.PageId == page.Id)
                .ToArray();

            foreach (Content incoming in page.Contents)
            {
                Content existing = existingItems.FirstOrDefault(predicate: item =>
                    item.Name == incoming.Name &&
                    item.CultureId == incoming.CultureId);

                incoming.Id = existing?.Id ?? 0;
                incoming.PageId = page.Id;

                _ = incoming.Id < 1
                    ? await contentProcessingService.AddContentAsync(newContent: incoming)
                    : await contentProcessingService.UpdateContentAsync(updatedContent: incoming);
            }
        }

    }, isValueTask: true);

    private static Page ValidatePage(Page page, string parameterName)
    {
        if (page == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return page;
    }
}