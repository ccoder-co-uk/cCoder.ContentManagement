// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ICommonObjectEventService
{
    ValueTask RaiseCommonObjectAddEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectUpdateEventAsync(CommonObject entity);

    ValueTask RaiseCommonObjectDeleteEventAsync(CommonObject entity);
}