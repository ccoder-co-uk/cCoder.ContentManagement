using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class ScriptReaderBroker(ICoreContextFactory coreContextFactory) : IScriptReaderBroker
{
    public IEnumerable<Script> GetScripts(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return (from script in coreDataContext.Scripts.IgnoreQueryFilters()
                where script.AppId == appId
                select script)
            .AsNoTracking()
            .ToArray();
    }

    public Script GetScript(int appId, string name)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        string lowerName = name.ToLowerInvariant();

        return coreDataContext.Scripts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefault(script =>
                script.AppId == appId
                && script.Name != null
                && script.Name.ToLower() == lowerName);
    }
}
