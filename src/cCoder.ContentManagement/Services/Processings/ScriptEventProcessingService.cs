// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ScriptEventProcessingService(IScriptEventService eventService)
    : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script script, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptAddEventAsync(inputs: [script, userId]);
        ValidateScript(script: script, parameterName: "entity");


        return eventService.RaiseScriptAddEventAsync(
            entity: script,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseScriptUpdateEventAsync(Script script, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptUpdateEventAsync(inputs: [script, userId]);
        ValidateScript(script: script, parameterName: "entity");


        return eventService.RaiseScriptUpdateEventAsync(
            entity: script,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaiseScriptDeleteEventAsync(Script script, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptDeleteEventAsync(inputs: [script, userId]);
        ValidateScript(script: script, parameterName: "entity");


        return eventService.RaiseScriptDeleteEventAsync(
            entity: script,
            userId: userId);

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