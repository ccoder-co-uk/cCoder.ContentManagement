// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IComponentEventProcessingService
{
    ValueTask RaiseComponentAddEventAsync(Component entity, string userId);

    ValueTask RaiseComponentUpdateEventAsync(Component entity, string userId);

    ValueTask RaiseComponentDeleteEventAsync(Component entity, string userId);
}