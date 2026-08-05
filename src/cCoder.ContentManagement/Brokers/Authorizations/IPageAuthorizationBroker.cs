// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal interface IPageAuthorizationBroker
{
    ValueTask<int?> GetAuthorizedPageIdAsync(
        string domain,
        string path);

    ValueTask<int?> GetPageIdIgnoringFiltersAsync(
        string domain,
        string path);
}