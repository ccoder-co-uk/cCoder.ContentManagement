// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class AppSupportingResourcesCoordinationService(
    IAppCultureOrchestrationService appCultureOrchestrationService,
    IScriptOrchestrationService scriptOrchestrationService,
    IResourceOrchestrationService resourceOrchestrationService) : IAppSupportingResourcesCoordinationService
{
    public ValueTask HandleAppAddAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppAddAsync(inputs: [app]);
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

    }, isValueTask: true);

    public ValueTask HandleAppUpdateAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppUpdateAsync(inputs: [app]);
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

    }, isValueTask: true);

    public ValueTask HandleAppDeleteAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppDeleteAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        await appCultureOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await scriptOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await resourceOrchestrationService.DeleteByAppIdAsync(appId: app.Id);

    }, isValueTask: true);

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

        AppCulture[] culturesToDelete = appCultureOrchestrationService.GetAllAppCulture(ignoreFilters: true)
            .Where(predicate: culture => culture.AppId == deletedApp.Id && !((ReadOnlySpan<string>)incomingCultureIds).Contains(value: culture.CultureId))
            .ToArray();

        if (culturesToDelete.Length > 0)
        {
            await appCultureOrchestrationService.DeleteAllAppCultureAsync(deletedAppCulture: culturesToDelete);
        }
    }

    private async ValueTask DeleteMissingResourcesAsync(App deletedApp)
    {
        int[] incomingResourceIds = deletedApp.Resources
            .Where(predicate: resource => resource.Id > 0)
            .Select(selector: resource => resource.Id)
            .ToArray();

        Resource[] resourcesToDelete = resourceOrchestrationService.GetAllResource(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingResourceIds).Contains(value: resource.Id))
            .ToArray();

        if (resourcesToDelete.Length > 0)
        {
            await resourceOrchestrationService.DeleteAllResourceAsync(deletedResource: resourcesToDelete);
        }
    }

    private async ValueTask DeleteMissingScriptsAsync(App deletedApp)
    {
        int[] incomingScriptIds = deletedApp.Scripts
            .Where(predicate: script => script.Id > 0)
            .Select(selector: script => script.Id)
            .ToArray();

        Script[] scriptsToDelete = scriptOrchestrationService.GetAllScript(ignoreFilters: true)
            .Where(predicate: script => script.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingScriptIds).Contains(value: script.Id))
            .ToArray();

        if (scriptsToDelete.Length > 0)
        {
            await scriptOrchestrationService.DeleteAllScriptAsync(deletedScript: scriptsToDelete);
        }
    }

    private async ValueTask AddOrUpdateCulturesAsync(App newApp)
    {
        HashSet<string> existingCultureIds = appCultureOrchestrationService.GetAllAppCulture(ignoreFilters: true)
            .Where(predicate: culture => culture.AppId == newApp.Id)
            .Select(selector: culture => culture.CultureId)
            .ToHashSet(comparer: StringComparer.Ordinal);

        foreach (AppCulture culture in newApp.Cultures)
        {
            if (!existingCultureIds.Contains(item: culture.CultureId))
            {
                await appCultureOrchestrationService.AddAppCultureAsync(newAppCulture: culture);
            }
        }
    }

    private async ValueTask AddOrUpdateResourcesAsync(App newApp)
    {
        HashSet<int> existingResourceIds = resourceOrchestrationService.GetAllResource(ignoreFilters: true)
            .Where(predicate: resource => resource.AppId == newApp.Id)
            .Select(selector: resource => resource.Id)
            .ToHashSet();

        foreach (Resource resource in newApp.Resources)
        {
            if (existingResourceIds.Contains(item: resource.Id))
            {
                await resourceOrchestrationService.UpdateResourceAsync(updatedResource: resource);
            }
            else
            {
                await resourceOrchestrationService.AddResourceAsync(newResource: resource);
            }
        }
    }

    private async ValueTask AddOrUpdateScriptsAsync(App newApp)
    {
        HashSet<int> existingScriptIds = scriptOrchestrationService.GetAllScript(ignoreFilters: true)
            .Where(predicate: script => script.AppId == newApp.Id)
            .Select(selector: script => script.Id)
            .ToHashSet();

        foreach (Script script in newApp.Scripts)
        {
            if (existingScriptIds.Contains(item: script.Id))
            {
                await scriptOrchestrationService.UpdateScriptAsync(updatedScript: script);
            }
            else
            {
                await scriptOrchestrationService.AddScriptAsync(newScript: script);
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