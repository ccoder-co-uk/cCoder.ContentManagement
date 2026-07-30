// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IResourceEventService
{
    ValueTask RaiseResourceAddEventAsync(Resource entity);

    ValueTask RaiseResourceUpdateEventAsync(Resource entity);

    ValueTask RaiseResourceDeleteEventAsync(Resource entity);
}