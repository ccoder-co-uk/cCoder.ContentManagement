// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using Microsoft.AspNetCore.OData.Results;

namespace cCoder.ContentManagement.Brokers.OData;

public interface IODataResultBroker
{
    SingleResult<T> CreateSingleResult<T>(IQueryable<T> queryable);
}