// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ScriptEventProcessingService(IScriptEventService eventService) : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script script) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptAddEventAsync(inputs: [script]);
        ValidateScript(script: script, parameterName: "entity");

        return eventService.RaiseScriptAddEventAsync(entity: script);

    }, isValueTask: true);

    public ValueTask RaiseScriptUpdateEventAsync(Script script) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptUpdateEventAsync(inputs: [script]);
        ValidateScript(script: script, parameterName: "entity");

        return eventService.RaiseScriptUpdateEventAsync(entity: script);

    }, isValueTask: true);

    public ValueTask RaiseScriptDeleteEventAsync(Script script) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptDeleteEventAsync(inputs: [script]);
        ValidateScript(script: script, parameterName: "entity");

        return eventService.RaiseScriptDeleteEventAsync(entity: script);

    }, isValueTask: true);

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