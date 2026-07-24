// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IContentEventService
{
    ValueTask RaiseContentAddEventAsync(Content entity);

    ValueTask RaiseContentUpdateEventAsync(Content entity);

    ValueTask RaiseContentDeleteEventAsync(Content entity);
}