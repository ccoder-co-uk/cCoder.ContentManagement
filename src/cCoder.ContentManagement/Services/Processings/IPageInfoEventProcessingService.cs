// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageInfoEventProcessingService
{
    ValueTask RaisePageInfoAddEventAsync(PageInfo entity, string userId);

    ValueTask RaisePageInfoUpdateEventAsync(PageInfo entity, string userId);

    ValueTask RaisePageInfoDeleteEventAsync(PageInfo entity, string userId);
}