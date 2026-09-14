// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class AppPageOrderOrchestrationService(
    IPageProcessingService pageProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IAppPageOrderOrchestrationService
{
    public ValueTask UpdatePageOrderAppAsync(int appId, App updatedApp) =>
        TryCatch(operation: async () =>
    {
        ValidatePageOrderAppOnUpdate(inputs: [appId, updatedApp]);
        ArgumentOutOfRangeException.ThrowIfLessThan(value: appId, other: 1);
        ArgumentNullException.ThrowIfNull(argument: updatedApp);

        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    Privilege = "app_update"
                }
            });

        Dictionary<int, Page> incomingPagesById = (updatedApp.Pages ?? [])
            .ToDictionary(keySelector: page => page.Id);

        Page[] existingPages = pageProcessingService
            .GetAllPage(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToArray();

        foreach (Page existingPage in existingPages)
        {
            if (incomingPagesById.TryGetValue(
                key: existingPage.Id,
                value: out Page incomingPage))
            {
                existingPage.Order = incomingPage.Order;
                existingPage.ParentId = incomingPage.ParentId;

                await pageProcessingService.UpdatePageAsync(
                    updatedPage: existingPage);
            }
        }
    }, isValueTask: true);
}