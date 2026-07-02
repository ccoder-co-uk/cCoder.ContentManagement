using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptEventProcessingService(IScriptEventService eventService) : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script entity)
    {
        ValidateScript(entity, "entity");

        return eventService.RaiseScriptAddEventAsync(entity);
    }

    public ValueTask RaiseScriptUpdateEventAsync(Script entity)
    {
        ValidateScript(entity, "entity");

        return eventService.RaiseScriptUpdateEventAsync(entity);
    }

    public ValueTask RaiseScriptDeleteEventAsync(Script entity)
    {
        ValidateScript(entity, "entity");

        return eventService.RaiseScriptDeleteEventAsync(entity);
    }

    private static void ValidateScript(Script script, string parameterName) =>
        ThrowIf(script == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
