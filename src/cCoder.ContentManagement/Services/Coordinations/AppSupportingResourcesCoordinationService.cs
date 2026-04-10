using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using App = cCoder.Data.Models.CMS.App;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Services.Coordinations;

internal class AppSupportingResourcesCoordinationService(
    IAppCultureBroker appCultureBroker,
    IScriptBroker scriptBroker,
    IResourceBroker resourceBroker) : IAppSupportingResourcesCoordinationService
{
    public async ValueTask HandleAppAddAsync(App app)
    {
        ValidateApp(app, "app");
        StampChildrenWithApp(app);
        await AddOrUpdateCulturesAsync(app);
        await AddOrUpdateResourcesAsync(app);
        await AddOrUpdateScriptsAsync(app);
    }

    public async ValueTask HandleAppUpdateAsync(App app)
    {
        ValidateApp(app, "app");
        StampChildrenWithApp(app);
        await DeleteMissingCulturesAsync(app);
        await DeleteMissingResourcesAsync(app);
        await DeleteMissingScriptsAsync(app);
        await AddOrUpdateCulturesAsync(app);
        await AddOrUpdateResourcesAsync(app);
        await AddOrUpdateScriptsAsync(app);
    }

    public async ValueTask HandleAppDeleteAsync(App app)
    {
        ValidateApp(app, "app");
        cCoder.Data.Models.CMS.AppCulture[] culturesToDelete = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id)
            .ToArray();
        cCoder.Data.Models.CMS.Script[] scriptsToDelete = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id)
            .ToArray();
        cCoder.Data.Models.CMS.Resource[] resourcesToDelete = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id)
            .ToArray();

        if (culturesToDelete.Length > 0)
        {
            await appCultureBroker.DeleteAllAppCulturesAsync(culturesToDelete);
        }

        if (scriptsToDelete.Length > 0)
        {
            await scriptBroker.DeleteAllScriptsAsync(scriptsToDelete);
        }

        if (resourcesToDelete.Length > 0)
        {
            await resourceBroker.DeleteAllResourcesAsync(resourcesToDelete);
        }
    }

    private static void StampChildrenWithApp(App app)
    {
        foreach (AppCulture item in app.Cultures ?? new List<AppCulture>())
        {
            item.AppId = app.Id;
        }
        foreach (Script item2 in app.Scripts ?? new List<Script>())
        {
            item2.AppId = app.Id;
        }
        foreach (Resource item3 in app.Resources ?? new List<Resource>())
        {
            item3.AppId = app.Id;
        }
    }

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return app;
    }

    private async ValueTask DeleteMissingCulturesAsync(App app)
    {
        string[] incomingCultureIds = (app.Cultures ?? Array.Empty<AppCulture>())
            .Select(culture => culture.CultureId)
            .ToArray();

        cCoder.Data.Models.CMS.AppCulture[] culturesToDelete = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id && !((ReadOnlySpan<string>)incomingCultureIds).Contains(culture.CultureId))
            .ToArray();

        if (culturesToDelete.Length > 0)
        {
            await appCultureBroker.DeleteAllAppCulturesAsync(culturesToDelete);
        }
    }

    private async ValueTask DeleteMissingResourcesAsync(App app)
    {
        int[] incomingResourceIds = (app.Resources ?? Array.Empty<Resource>())
            .Where(resource => resource.Id > 0)
            .Select(resource => resource.Id)
            .ToArray();

        cCoder.Data.Models.CMS.Resource[] resourcesToDelete = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id && !((ReadOnlySpan<int>)incomingResourceIds).Contains(resource.Id))
            .ToArray();

        if (resourcesToDelete.Length > 0)
        {
            await resourceBroker.DeleteAllResourcesAsync(resourcesToDelete);
        }
    }

    private async ValueTask DeleteMissingScriptsAsync(App app)
    {
        int[] incomingScriptIds = (app.Scripts ?? Array.Empty<Script>())
            .Where(script => script.Id > 0)
            .Select(script => script.Id)
            .ToArray();

        cCoder.Data.Models.CMS.Script[] scriptsToDelete = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id && !((ReadOnlySpan<int>)incomingScriptIds).Contains(script.Id))
            .ToArray();

        if (scriptsToDelete.Length > 0)
        {
            await scriptBroker.DeleteAllScriptsAsync(scriptsToDelete);
        }
    }

    private async ValueTask AddOrUpdateCulturesAsync(App app)
    {
        HashSet<string> existingCultureIds = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id)
            .Select(culture => culture.CultureId)
            .ToHashSet(StringComparer.Ordinal);

        foreach (AppCulture culture in app.Cultures ?? Array.Empty<AppCulture>())
        {
            if (!existingCultureIds.Contains(culture.CultureId))
            {
                await appCultureBroker.AddAppCultureAsync(culture);
            }
        }
    }

    private async ValueTask AddOrUpdateResourcesAsync(App app)
    {
        HashSet<int> existingResourceIds = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id)
            .Select(resource => resource.Id)
            .ToHashSet();

        foreach (Resource resource in app.Resources ?? Array.Empty<Resource>())
        {
            if (existingResourceIds.Contains(resource.Id))
            {
                await resourceBroker.UpdateResourceAsync(resource);
            }
            else
            {
                await resourceBroker.AddResourceAsync(resource);
            }
        }
    }

    private async ValueTask AddOrUpdateScriptsAsync(App app)
    {
        HashSet<int> existingScriptIds = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id)
            .Select(script => script.Id)
            .ToHashSet();

        foreach (Script script in app.Scripts ?? Array.Empty<Script>())
        {
            if (existingScriptIds.Contains(script.Id))
            {
                await scriptBroker.UpdateScriptAsync(script);
            }
            else
            {
                await scriptBroker.AddScriptAsync(script);
            }
        }
    }
}
