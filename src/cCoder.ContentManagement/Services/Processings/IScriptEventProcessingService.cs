// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IScriptEventProcessingService
{
    ValueTask RaiseScriptAddEventAsync(Script entity);

    ValueTask RaiseScriptUpdateEventAsync(Script entity);

    ValueTask RaiseScriptDeleteEventAsync(Script entity);
}