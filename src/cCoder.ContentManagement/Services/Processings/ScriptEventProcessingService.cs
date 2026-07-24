// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class ScriptEventProcessingService(IScriptEventService eventService) : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptAddEventAsync(inputs: [entity]);
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptAddEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseScriptUpdateEventAsync(Script entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptUpdateEventAsync(inputs: [entity]);
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptUpdateEventAsync(entity: entity);

    }, isValueTask: true);

    public ValueTask RaiseScriptDeleteEventAsync(Script entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseScriptDeleteEventAsync(inputs: [entity]);
        ValidateScript(script: entity, parameterName: "entity");

        return eventService.RaiseScriptDeleteEventAsync(entity: entity);

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