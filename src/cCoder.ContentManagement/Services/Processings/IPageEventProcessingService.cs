// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPageEventProcessingService
{
    ValueTask RaisePageAddEventAsync(Page entity);

    ValueTask RaisePageUpdateEventAsync(Page entity);

    ValueTask RaisePageDeleteEventAsync(Page entity);
}