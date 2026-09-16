// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Caching;

internal interface ICacheBroker
{
    T Get<T>(string key);

    void Set<T>(string key, T value, TimeSpan expiry);
}