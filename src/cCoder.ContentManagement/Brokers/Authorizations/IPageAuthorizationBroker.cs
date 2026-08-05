// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Authorizations;

using cCoder.ContentManagement.Models;

internal interface IPageAuthorizationBroker
{
    ValueTask<PageAuthorizationResult> GetAuthorizedPageAsync(
        string domain,
        string path);

    ValueTask<PageAuthorizationResult> GetPageIgnoringFiltersAsync(
        string domain,
        string path);

    ValueTask<bool> CanUpdatePageAsync(
        int appId,
        int pageId);
}