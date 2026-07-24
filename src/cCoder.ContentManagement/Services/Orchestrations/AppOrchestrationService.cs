// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Brokers;
using Microsoft.EntityFrameworkCore;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class AppOrchestrationService(
    IAppProcessingService processingService,
    IAppEventProcessingService eventService,
    IAuthorizationBroker authorizationBroker) : IAppOrchestrationService
{
    public App Get(int id)
    {
        ValidateId(id: id, parameterName: "id");
        return processingService.Get(id: id);
    }

    public App GetByDomain(string domain, bool ignoreFilters = false)
    {
        ValidateDomain(domain: domain, parameterName: "domain");
        return processingService.GetByDomain(domain: domain, ignoreFilters: ignoreFilters);
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<App> AddAsync(App entity)
    {
        ValidateApp(app: entity, parameterName: "entity");
        App result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseAppAddEventAsync(app: result);
        return result;
    }

    public async ValueTask<App> UpdateAsync(App entity)
    {
        ValidateApp(app: entity, parameterName: "entity");
        App result = await processingService.UpdateAsync(entity: entity);
        ReflectUpdatedApp(source: result, target: entity);
        await eventService.RaiseAppUpdateEventAsync(app: entity);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        App app = processingService.GetAll(ignoreFilters: true)
            .Include(navigationPropertyPath: foundApp => foundApp.Roles)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == id);

        if (app?.Roles?.Any() == true)
        {
            authorizationBroker.Authorize(appId: id, privilege: "app_delete");
        }

        if (app != null)
        {
            await eventService.RaiseAppDeleteEventAsync(app: app);
        }

        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<App>>> AddOrUpdate(IEnumerable<App> items) =>
        processingService.AddOrUpdate(items: ValidateApps(apps: items, parameterName: "items"));

    public ValueTask DeleteAllAsync(IEnumerable<App> items) =>
        processingService.DeleteAllAsync(items: ValidateApps(apps: items, parameterName: "items"));

    public IQueryable<User> GetAppUsers(int appId)
    {
        ValidateId(id: appId, parameterName: "appId");
        return processingService.GetAppUsers(appId: appId);
    }

    public ValueTask UpdatePageOrderAsync(int key, App app) =>
        processingService.UpdatePageOrderAsync(key: key, app: ValidateApp(app: app, parameterName: "app"));

    public App ResolveCurrentApp() =>
        processingService.ResolveCurrentApp();

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(condition: id < 1, message: parameterName + " must be greater than 0.");

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return app;
    }

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: domain), message: parameterName + " is required.");

    private static IEnumerable<App> ValidateApps(IEnumerable<App> apps, string parameterName)
    {
        if (apps == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return apps;
    }

    private static void ReflectUpdatedApp(App source, App target)
    {
        target.Id = source.Id;
        target.DefaultCultureId = source.DefaultCultureId;
        target.TenantId = source.TenantId;
        target.Name = source.Name;
        target.Domain = source.Domain;
        target.DefaultTheme = source.DefaultTheme;
        target.ConfigJson = source.ConfigJson;
        target.Cultures = source.Cultures;
        target.Pages = source.Pages;
        target.Components = source.Components;
        target.Scripts = source.Scripts;
        target.Roles = source.Roles;
        target.Templates = source.Templates;
        target.Resources = source.Resources;
        target.Layouts = source.Layouts;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}