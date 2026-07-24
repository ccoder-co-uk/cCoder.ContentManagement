// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class AppRenderableCoordinationService(
    ITemplateOrchestrationService templateOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService) : IAppRenderableCoordinationService
{
    public ValueTask HandleAppAddAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppAddAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Templates != null)
        {
            await templateOrchestrationService.AddOrUpdateTemplateResult(newTemplate: app.Templates);
        }

        if (app.Layouts != null)
        {
            await layoutOrchestrationService.AddOrUpdateLayoutResult(newLayout: app.Layouts);
        }

    }, isValueTask: true);

    public ValueTask HandleAppUpdateAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppUpdateAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Templates != null)
        {
            await DeleteMissingTemplatesAsync(deletedApp: app);
            await templateOrchestrationService.AddOrUpdateTemplateResult(newTemplate: app.Templates);
        }

        if (app.Layouts != null)
        {
            await DeleteMissingLayoutsAsync(deletedApp: app);
            await layoutOrchestrationService.AddOrUpdateLayoutResult(newLayout: app.Layouts);
        }

    }, isValueTask: true);

    public ValueTask HandleAppDeleteAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppDeleteAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        await templateOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await layoutOrchestrationService.DeleteByAppIdAsync(appId: app.Id);

    }, isValueTask: true);

    private static void StampChildrenWithApp(App app)
    {
        if (app.Templates != null)
        {
            foreach (Template template in app.Templates)
            {
                template.AppId = app.Id;
            }
        }

        if (app.Layouts != null)
        {
            foreach (Layout layout in app.Layouts)
            {
                layout.AppId = app.Id;
            }
        }
    }

    private async ValueTask DeleteMissingTemplatesAsync(App deletedApp)
    {
        int[] incomingTemplateIds = deletedApp.Templates
            .Where(predicate: template => template.Id > 0)
            .Select(selector: template => template.Id)
            .ToArray();

        Template[] templatesToDelete = templateOrchestrationService.GetAllTemplate(ignoreFilters: true)
            .Where(predicate: template => template.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingTemplateIds).Contains(value: template.Id))
            .ToArray();

        if (templatesToDelete.Length > 0)
        {
            await templateOrchestrationService.DeleteAllTemplateAsync(deletedTemplate: templatesToDelete);
        }
    }

    private async ValueTask DeleteMissingLayoutsAsync(App deletedApp)
    {
        int[] incomingLayoutIds = deletedApp.Layouts
            .Where(predicate: layout => layout.Id > 0)
            .Select(selector: layout => layout.Id)
            .ToArray();

        Layout[] layoutsToDelete = layoutOrchestrationService.GetAllLayout(ignoreFilters: true)
            .Where(predicate: layout => layout.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingLayoutIds).Contains(value: layout.Id))
            .ToArray();

        if (layoutsToDelete.Length > 0)
        {
            await layoutOrchestrationService.DeleteAllLayoutAsync(deletedLayout: layoutsToDelete);
        }
    }

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(condition: app == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}