// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentEventProcessingService
{
    ValueTask RaiseComponentAddEventAsync(Component entity);

    ValueTask RaiseComponentUpdateEventAsync(Component entity);

    ValueTask RaiseComponentDeleteEventAsync(Component entity);
}