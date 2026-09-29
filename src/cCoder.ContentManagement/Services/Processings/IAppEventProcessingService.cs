// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAppEventProcessingService
{
    ValueTask RaiseAppAddEventAsync(App app, string userId);

    ValueTask RaiseAppDeleteEventAsync(App app, string userId);

    ValueTask RaiseAppUpdateEventAsync(App app, string userId);
}