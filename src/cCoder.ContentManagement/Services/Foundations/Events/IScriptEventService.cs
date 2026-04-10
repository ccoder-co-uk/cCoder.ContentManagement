using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IScriptEventService
{
    ValueTask RaiseScriptAddEventAsync(Script entity);

    ValueTask RaiseScriptUpdateEventAsync(Script entity);

    ValueTask RaiseScriptDeleteEventAsync(Script entity);
}
