// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PagePackageImportCoordinationService(
    IPageOrchestrationService pageOrchestrationService,
    IPageImportOrchestrationService pageImportOrchestrationService)
    : IPagePackageImportCoordinationService
{
    public ValueTask ImportPagesAsync(int appId, Page[] pages) =>
        TryCatch(operation: async () =>
    {
        ValidateImportPagesAsync(inputs: [appId, pages]);

        Page[] importedPages = await pageOrchestrationService.ImportPagesAsync(
            appId: appId,
            items: pages);

        foreach (Page page in importedPages)
        {
            await pageImportOrchestrationService.HandlePageImportAsync(
                page: page);
        }

    }, isValueTask: true);
}