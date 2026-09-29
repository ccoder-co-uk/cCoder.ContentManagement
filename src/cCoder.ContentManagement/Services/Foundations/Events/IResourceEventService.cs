// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IResourceEventService
{
    ValueTask RaiseResourceAddEventAsync(Resource entity, string userId);

    ValueTask RaiseResourceUpdateEventAsync(Resource entity, string userId);

    ValueTask RaiseResourceDeleteEventAsync(Resource entity, string userId);
}