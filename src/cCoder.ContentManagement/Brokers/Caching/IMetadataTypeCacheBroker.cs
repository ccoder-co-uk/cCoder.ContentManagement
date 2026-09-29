// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
namespace cCoder.ContentManagement.Brokers.Caching;

internal interface IMetadataTypeCacheBroker
{
    IEnumerable<string> GetAll();
}