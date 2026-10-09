// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
namespace cCoder.ContentManagement.Brokers.Authorizations;

using cCoder.ContentManagement.Models;

internal interface IPageAuthorizationBroker
{
    ValueTask<PageAuthorizationData> GetAuthorizedPageAsync(
        string domain,
        string path);

    ValueTask<PageAuthorizationData> GetPageIgnoringFiltersAsync(
        string domain,
        string path);

    ValueTask<bool> CanUpdatePageAsync(
        int appId,
        int pageId);
}