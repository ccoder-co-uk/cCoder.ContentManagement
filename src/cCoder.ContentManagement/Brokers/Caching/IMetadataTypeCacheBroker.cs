// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers.Caching;

internal interface IMetadataTypeCacheBroker
{
    IEnumerable<string> GetAll();
}