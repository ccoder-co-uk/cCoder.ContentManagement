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
        ValidateId(id, "id");
        return processingService.Get(id);
    }

    public App GetByDomain(string domain, bool ignoreFilters = false)
    {
        ValidateDomain(domain, "domain");
        return processingService.GetByDomain(domain, ignoreFilters);
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<App> AddAsync(App entity)
    {
        ValidateApp(entity, "entity");
        App result = await processingService.AddAsync(entity);
        await eventService.RaiseAppAddEventAsync(result);
        return result;
    }

    public async ValueTask<App> UpdateAsync(App entity)
    {
        ValidateApp(entity, "entity");
        App result = await processingService.UpdateAsync(entity);
        ReflectUpdatedApp(result, entity);
        await eventService.RaiseAppUpdateEventAsync(entity);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        App app = processingService.GetAll(ignoreFilters: true)
            .Include(foundApp => foundApp.Roles)
            .FirstOrDefault(foundApp => foundApp.Id == id);

        if (app?.Roles?.Any() == true)
            authorizationBroker.Authorize(id, "app_delete");

        if (app != null)
            await eventService.RaiseAppDeleteEventAsync(app);

        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<App>>> AddOrUpdate(IEnumerable<App> items) =>
        processingService.AddOrUpdate(ValidateApps(items, "items"));

    public ValueTask DeleteAllAsync(IEnumerable<App> items) =>
        processingService.DeleteAllAsync(ValidateApps(items, "items"));

    public IQueryable<User> GetAppUsers(int appId)
    {
        ValidateId(appId, "appId");
        return processingService.GetAppUsers(appId);
    }

    public ValueTask UpdatePageOrderAsync(int key, App app) =>
        processingService.UpdatePageOrderAsync(key, ValidateApp(app, "app"));

    public App ResolveCurrentApp() => processingService.ResolveCurrentApp();

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
            throw new ValidationException(parameterName + " is required.");

        return app;
    }

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(string.IsNullOrWhiteSpace(domain), parameterName + " is required.");

    private static IEnumerable<App> ValidateApps(IEnumerable<App> apps, string parameterName)
    {
        if (apps == null)
            throw new ValidationException(parameterName + " is required.");

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
            throw new ValidationException(message);
    }
}
