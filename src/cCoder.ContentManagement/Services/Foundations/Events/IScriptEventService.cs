using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface IScriptEventService
{
    ValueTask RaiseScriptAddEventAsync(Script entity);

    ValueTask RaiseScriptUpdateEventAsync(Script entity);

    ValueTask RaiseScriptDeleteEventAsync(Script entity);
}
