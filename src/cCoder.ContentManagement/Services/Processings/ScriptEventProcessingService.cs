using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Events;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Processings;

internal class ScriptEventProcessingService(IScriptEventService eventService) : IScriptEventProcessingService
{
    public ValueTask RaiseScriptAddEventAsync(Script entity)
    {
        return eventService.RaiseScriptAddEventAsync(ValidateScript(entity, "entity"));
    }

    public ValueTask RaiseScriptUpdateEventAsync(Script entity)
    {
        return eventService.RaiseScriptUpdateEventAsync(ValidateScript(entity, "entity"));
    }

    public ValueTask RaiseScriptDeleteEventAsync(Script entity)
    {
        return eventService.RaiseScriptDeleteEventAsync(ValidateScript(entity, "entity"));
    }

    private static Script ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return script;
    }
}
