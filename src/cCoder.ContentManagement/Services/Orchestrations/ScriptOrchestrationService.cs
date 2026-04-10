using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Services.Processings;
using Script = cCoder.Data.Models.CMS.Script;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Script>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class ScriptOrchestrationService(
    IScriptProcessingService processingService,
    IScriptEventProcessingService eventService) : IScriptOrchestrationService
{
    public Script Get(int id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Script> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Script> AddAsync(Script entity)
    {
        ValidateScript(entity, "entity");

        Script result = await processingService.AddAsync(entity);
        await eventService.RaiseScriptAddEventAsync(result);
        return result;
    }

    public async ValueTask<Script> UpdateAsync(Script entity)
    {
        ValidateScript(entity, "entity");

        Script result = await processingService.UpdateAsync(entity);
        await eventService.RaiseScriptUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");

        Script entity;
        try
        {
            entity = processingService.Get(id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(script => script.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseScriptDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        ValidateAppId(appId, "appId");
        Script[] scriptsToDelete = [.. GetAll(ignoreFilters: true).Where(script => script.AppId == appId)];

        if (scriptsToDelete.Length > 0)
        {
            await DeleteAllAsync(scriptsToDelete);
        }
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Script> items)
    {
        Script[] scripts = ValidateScripts(items, "items").ToArray();
        List<Result> results = new();

        foreach (Script script in scripts)
        {
            try
            {
                Script result = script.Id <= 0
                    ? await AddAsync(script)
                    : await UpdateAsync(script);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = script.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
                {
                    Success = false,
                    Item = script,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask ImportScriptsAsync(int appId, Script[] items)
    {
        ValidateAppId(appId, "appId");

        Script[] validatedItems = ValidateScripts(items, "items").ToArray();
        string[] names = validatedItems.Select(script => script.Name.ToLower()).ToArray();

        var dbVersions = (from script in processingService.GetAll()
                          where script.AppId == appId && ((ReadOnlySpan<string>)names).Contains(script.Name.ToLower())
                          select new { script.Id, script.Name }).ToArray();

        Array.ForEach(validatedItems, script =>
        {
            script.AppId = appId;
            script.Id = dbVersions.FirstOrDefault(existing =>
                existing.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        });

        await AddOrUpdate(validatedItems);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Script> items)
    {
        Script[] scripts = ValidateScripts(items, "items").ToArray();

        foreach (Script script in scripts)
        {
            await DeleteAsync(script.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return id;
    }

    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static Script ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return script;
    }

    private static IEnumerable<Script> ValidateScripts(IEnumerable<Script> scripts, string parameterName)
    {
        if (scripts == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return scripts;
    }
}
