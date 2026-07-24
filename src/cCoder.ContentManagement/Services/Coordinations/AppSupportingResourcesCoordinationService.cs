// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal class AppSupportingResourcesCoordinationService(
    IAppCultureBroker appCultureBroker,
    IScriptBroker scriptBroker,
    IResourceBroker resourceBroker,
    IAppCultureOrchestrationService appCultureOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService) : IAppSupportingResourcesCoordinationService
{
    public async ValueTask HandleAppAddAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Cultures != null)
        {
            await AddOrUpdateCulturesAsync(newApp: app);
        }

        if (app.Resources != null)
        {
            await AddOrUpdateResourcesAsync(newApp: app);
        }

        if (app.Scripts != null)
        {
            await AddOrUpdateScriptsAsync(newApp: app);
        }
    }

    public async ValueTask HandleAppUpdateAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Cultures != null)
        {
            await DeleteMissingCulturesAsync(deletedApp: app);
            await AddOrUpdateCulturesAsync(newApp: app);
        }

        if (app.Resources != null)
        {
            await DeleteMissingResourcesAsync(deletedApp: app);
            await AddOrUpdateResourcesAsync(newApp: app);
        }

        if (app.Scripts != null)
        {
            await DeleteMissingScriptsAsync(deletedApp: app);
            await AddOrUpdateScriptsAsync(newApp: app);
        }
    }

    public async ValueTask HandleAppDeleteAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        await appCultureOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await scriptOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await resourceOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
    }

    private static void StampChildrenWithApp(App app)
    {
        if (app.Cultures != null)
        {
            foreach (AppCulture item in app.Cultures)
            {
                item.AppId = app.Id;
            }
        }

        if (app.Scripts != null)
        {
            foreach (Script item2 in app.Scripts)
            {
                item2.AppId = app.Id;
            }
        }

        if (app.Resources != null)
        {
            foreach (Resource item3 in app.Resources)
            {
                item3.AppId = app.Id;
            }
        }
    }

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(condition: app == null, message: parameterName + " is required.");

    private async ValueTask DeleteMissingCulturesAsync(App deletedApp)
    {
        string[] incomingCultureIds = deletedApp.Cultures
            .Select(selector: culture => culture.CultureId)
            .ToArray();

        AppCulture[] culturesToDelete = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(predicate: culture => culture.AppId == deletedApp.Id && !((ReadOnlySpan<string>)incomingCultureIds).Contains(value: culture.CultureId))
            .ToArray();

        if (culturesToDelete.Length > 0)
        {
            await appCultureBroker.DeleteAllAppCulturesAsync(deletedAppCulture: culturesToDelete);
        }
    }

    private async ValueTask DeleteMissingResourcesAsync(App deletedApp)
    {
        int[] incomingResourceIds = deletedApp.Resources
            .Where(predicate: resource => resource.Id > 0)
            .Select(selector: resource => resource.Id)
            .ToArray();

        Resource[] resourcesToDelete = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingResourceIds).Contains(value: resource.Id))
            .ToArray();

        if (resourcesToDelete.Length > 0)
        {
            await resourceBroker.DeleteAllResourcesAsync(deletedResource: resourcesToDelete);
        }
    }

    private async ValueTask DeleteMissingScriptsAsync(App deletedApp)
    {
        int[] incomingScriptIds = deletedApp.Scripts
            .Where(predicate: script => script.Id > 0)
            .Select(selector: script => script.Id)
            .ToArray();

        Script[] scriptsToDelete = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(predicate: script => script.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingScriptIds).Contains(value: script.Id))
            .ToArray();

        if (scriptsToDelete.Length > 0)
        {
            await scriptBroker.DeleteAllScriptsAsync(deletedScript: scriptsToDelete);
        }
    }

    private async ValueTask AddOrUpdateCulturesAsync(App newApp)
    {
        HashSet<string> existingCultureIds = appCultureBroker.GetAllAppCultures(ignoreFilters: true)
            .Where(predicate: culture => culture.AppId == newApp.Id)
            .Select(selector: culture => culture.CultureId)
            .ToHashSet(comparer: StringComparer.Ordinal);

        foreach (AppCulture culture in newApp.Cultures)
        {
            if (!existingCultureIds.Contains(item: culture.CultureId))
            {
                await appCultureBroker.AddAppCultureAsync(newAppCulture: culture);
            }
        }
    }

    private async ValueTask AddOrUpdateResourcesAsync(App newApp)
    {
        HashSet<int> existingResourceIds = resourceBroker.GetAllResources(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == newApp.Id)
            .Select(selector: resource => resource.Id)
            .ToHashSet();

        foreach (Resource resource in newApp.Resources)
        {
            if (existingResourceIds.Contains(item: resource.Id))
            {
                await resourceBroker.UpdateResourceAsync(updatedResource: resource);
            }
            else
            {
                await resourceBroker.AddResourceAsync(newResource: resource);
            }
        }
    }

    private async ValueTask AddOrUpdateScriptsAsync(App newApp)
    {
        HashSet<int> existingScriptIds = scriptBroker.GetAllScripts(ignoreFilters: true)
            .Where(predicate: script => script.AppId == newApp.Id)
            .Select(selector: script => script.Id)
            .ToHashSet();

        foreach (Script script in newApp.Scripts)
        {
            if (existingScriptIds.Contains(item: script.Id))
            {
                await scriptBroker.UpdateScriptAsync(updatedScript: script);
            }
            else
            {
                await scriptBroker.AddScriptAsync(newScript: script);
            }
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}