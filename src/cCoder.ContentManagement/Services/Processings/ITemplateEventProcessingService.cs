// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ITemplateEventProcessingService
{
    ValueTask RaiseTemplateAddEventAsync(Template entity, string userId);

    ValueTask RaiseTemplateUpdateEventAsync(Template entity, string userId);

    ValueTask RaiseTemplateDeleteEventAsync(Template entity, string userId);
}