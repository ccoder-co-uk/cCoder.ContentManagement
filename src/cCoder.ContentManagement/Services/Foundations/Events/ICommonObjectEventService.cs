// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ICommonObjectEventService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity, string userId);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity, string userId);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity, string userId);

    ValueTask RaiseCommonObjectsImportedEventAsync(
        CommonObject[] commonObjects,
        string userId);
}