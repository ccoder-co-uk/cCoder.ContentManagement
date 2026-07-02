using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

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
        AppCulture[] culturesToDelete = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id)
            .ToArray();
        Script[] scriptsToDelete = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id)
            .ToArray();
        Resource[] resourcesToDelete = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id)
            .ToArray();

        if (culturesToDelete.Length > 0)
            await appCultureBroker.DeleteAllAppCulturesAsync(culturesToDelete);

        if (scriptsToDelete.Length > 0)
            await scriptBroker.DeleteAllScriptsAsync(scriptsToDelete);

        if (resourcesToDelete.Length > 0)
            await resourceBroker.DeleteAllResourcesAsync(resourcesToDelete);
    }

    private static void StampChildrenWithApp(App app)
    {
        if (app.Cultures != null)
            foreach (AppCulture item in app.Cultures)
                item.AppId = app.Id;

        if (app.Scripts != null)
            foreach (Script item2 in app.Scripts)
                item2.AppId = app.Id;

        if (app.Resources != null)
            foreach (Resource item3 in app.Resources)
                item3.AppId = app.Id;
    }

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(app == null, parameterName + " is required.");

    private async ValueTask DeleteMissingCulturesAsync(App app)
    {
        string[] incomingCultureIds = (app.Cultures ?? Array.Empty<AppCulture>())
            .Select(culture => culture.CultureId)
            .ToArray();

        AppCulture[] culturesToDelete = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id && !((ReadOnlySpan<string>)incomingCultureIds).Contains(culture.CultureId))
            .ToArray();

        if (culturesToDelete.Length > 0)
            await appCultureBroker.DeleteAllAppCulturesAsync(culturesToDelete);
    }

    private async ValueTask DeleteMissingResourcesAsync(App app)
    {
        int[] incomingResourceIds = (app.Resources ?? Array.Empty<Resource>())
            .Where(resource => resource.Id > 0)
            .Select(resource => resource.Id)
            .ToArray();

        Resource[] resourcesToDelete = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id && !((ReadOnlySpan<int>)incomingResourceIds).Contains(resource.Id))
            .ToArray();

        if (resourcesToDelete.Length > 0)
            await resourceBroker.DeleteAllResourcesAsync(resourcesToDelete);
    }

    private async ValueTask DeleteMissingScriptsAsync(App app)
    {
        int[] incomingScriptIds = (app.Scripts ?? Array.Empty<Script>())
            .Where(script => script.Id > 0)
            .Select(script => script.Id)
            .ToArray();

        Script[] scriptsToDelete = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id && !((ReadOnlySpan<int>)incomingScriptIds).Contains(script.Id))
            .ToArray();

        if (scriptsToDelete.Length > 0)
            await scriptBroker.DeleteAllScriptsAsync(scriptsToDelete);
    }

    private async ValueTask AddOrUpdateCulturesAsync(App app)
    {
        HashSet<string> existingCultureIds = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(culture => culture.AppId == app.Id)
            .Select(culture => culture.CultureId)
            .ToHashSet(StringComparer.Ordinal);

        if (app.Cultures != null)
            foreach (AppCulture culture in app.Cultures)
                if (!existingCultureIds.Contains(culture.CultureId))
                    await appCultureBroker.AddAppCultureAsync(culture);
    }

    private async ValueTask AddOrUpdateResourcesAsync(App app)
    {
        HashSet<int> existingResourceIds = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(resource => resource.AppId == app.Id)
            .Select(resource => resource.Id)
            .ToHashSet();

        if (app.Resources != null)
            foreach (Resource resource in app.Resources)
                if (existingResourceIds.Contains(resource.Id))
                    await resourceBroker.UpdateResourceAsync(resource);
                else
                    await resourceBroker.AddResourceAsync(resource);
    }

    private async ValueTask AddOrUpdateScriptsAsync(App app)
    {
        HashSet<int> existingScriptIds = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(script => script.AppId == app.Id)
            .Select(script => script.Id)
            .ToHashSet();

        if (app.Scripts != null)
            foreach (Script script in app.Scripts)
                if (existingScriptIds.Contains(script.Id))
                    await scriptBroker.UpdateScriptAsync(script);
                else
                    await scriptBroker.AddScriptAsync(script);
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
