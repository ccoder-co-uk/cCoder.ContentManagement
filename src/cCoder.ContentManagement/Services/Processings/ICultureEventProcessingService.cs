// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface ICultureEventProcessingService
{
    ValueTask RaiseCultureAddEventAsync(Culture entity);

    ValueTask RaiseCultureUpdateEventAsync(Culture entity);

    ValueTask RaiseCultureDeleteEventAsync(Culture entity);
}