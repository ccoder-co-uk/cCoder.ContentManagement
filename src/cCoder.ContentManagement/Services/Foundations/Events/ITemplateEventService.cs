// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ITemplateEventService
{
    ValueTask RaiseTemplateAddEventAsync(Template entity);

    ValueTask RaiseTemplateUpdateEventAsync(Template entity);

    ValueTask RaiseTemplateDeleteEventAsync(Template entity);
}