// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ICommonObjectEventProcessingService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity);
}