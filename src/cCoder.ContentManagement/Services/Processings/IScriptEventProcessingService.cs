using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Processings;

public interface IScriptEventProcessingService
{
    ValueTask RaiseScriptAddEventAsync(Script entity);

    ValueTask RaiseScriptUpdateEventAsync(Script entity);

    ValueTask RaiseScriptDeleteEventAsync(Script entity);
}
