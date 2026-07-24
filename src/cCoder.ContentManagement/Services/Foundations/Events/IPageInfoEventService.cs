// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IPageInfoEventService
{
    ValueTask RaisePageInfoAddEventAsync(PageInfo entity);

    ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity);

    ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity);
}