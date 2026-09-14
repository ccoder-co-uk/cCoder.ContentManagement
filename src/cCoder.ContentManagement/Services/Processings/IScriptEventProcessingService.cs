// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IScriptEventProcessingService
{
    ValueTask RaiseScriptAddEventAsync(Script entity, string userId);

    ValueTask RaiseScriptUpdateEventAsync(Script entity, string userId);

    ValueTask RaiseScriptDeleteEventAsync(Script entity, string userId);
}