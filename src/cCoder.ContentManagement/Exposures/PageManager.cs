// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class PageManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : IPageManager
{
    public IQueryable<Page> GetAll() =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            IQueryable<Page>>(
                name: "Page",
                operation: service => service.GetAllPage());

    public Page Get(int pageId) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            Page>(
                name: "Page",
                operation: service => service.GetPage(
                    pageId: pageId));

    public Page GetRoot(int pageId) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            Page>(
                name: "Page",
                operation: service => service.GetRootPage(
                    pageId: pageId));

    public string GetMenu(int pageId, string culture) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            string>(
                name: "Page",
                operation: service => service.MenuFor(
                    pageId: pageId,
                    culture: culture));

    public ValueTask<Page> AddAsync(Page newPage) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            ValueTask<Page>>(
                name: "Page",
                operation: service => service.AddPageAsync(
                    newPage: newPage));

    public ValueTask<Page> UpdateAsync(Page updatedPage) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            ValueTask<Page>>(
                name: "Page",
                operation: service => service.UpdatePageAsync(
                    updatedPage: updatedPage));

    public ValueTask DeleteAsync(int pageId) =>
        serviceProviderExecutionService.Execute<
            IPageOrchestrationService,
            ValueTask>(
                name: "Page",
                operation: service => service.DeleteAsync(
                    pageId: pageId));
}