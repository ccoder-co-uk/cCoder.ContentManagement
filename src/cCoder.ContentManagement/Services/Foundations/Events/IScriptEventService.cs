// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IScriptEventService
{
    ValueTask RaiseScriptAddEventAsync(Script entity, string userId);

    ValueTask RaiseScriptUpdateEventAsync(Script entity, string userId);

    ValueTask RaiseScriptDeleteEventAsync(Script entity, string userId);
}