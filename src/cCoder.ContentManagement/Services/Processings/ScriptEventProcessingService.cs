// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptEventProcessingService(IScriptEventService eventService) : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptAddEventAsync(entity: entity);
    }

    public ValueTask RaiseScriptUpdateEventAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptUpdateEventAsync(entity: entity);
    }

    public ValueTask RaiseScriptDeleteEventAsync(Script entity)
    {
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptDeleteEventAsync(entity: entity);
    }

    private static void ValidateScript(Script script, string parameterName) =>
        ThrowIf(condition: script == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}