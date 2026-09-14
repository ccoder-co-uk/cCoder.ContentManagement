// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ILayoutEventProcessingService
{
    ValueTask RaiseLayoutAddEventAsync(Layout entity, string userId);

    ValueTask RaiseLayoutUpdateEventAsync(Layout entity, string userId);

    ValueTask RaiseLayoutDeleteEventAsync(Layout entity, string userId);
}