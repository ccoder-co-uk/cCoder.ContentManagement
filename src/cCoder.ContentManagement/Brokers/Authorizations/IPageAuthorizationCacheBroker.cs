// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Brokers.Authorizations;

internal interface IPageAuthorizationCacheBroker
{
    PageAuthorizationData Get(string key);

    void Set(
        string key,
        PageAuthorizationData pageAuthorizationData);

    void Clear();
}