// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface ICommonObjectEventOrchestrationService
{
    ValueTask RaiseCommonObjectsImportedEventAsync(
        IEnumerable<OperationResult<CommonObject>> results);

    ValueTask RaiseCommonObjectUpdatedEventAsync(CommonObject commonObject);

    ValueTask RaiseCommonObjectDeletedEventAsync(CommonObject commonObject);
}