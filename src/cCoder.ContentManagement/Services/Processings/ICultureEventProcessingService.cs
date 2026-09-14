// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ICultureEventProcessingService
{
    ValueTask RaiseCultureAddEventAsync(Culture entity, string userId);

    ValueTask RaiseCultureUpdateEventAsync(Culture entity, string userId);

    ValueTask RaiseCultureDeleteEventAsync(Culture entity, string userId);
}