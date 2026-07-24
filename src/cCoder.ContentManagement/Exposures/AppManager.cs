// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures;

internal sealed class AppManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : IAppManager
{
    public App Get(int appManagerId, bool ignoreFilters = false) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            App>(
                name: "App",
                operation: service => service.GetApp(
                    appId: appManagerId));

    public App GetByDomain(string domain, bool ignoreFilters = false) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            App>(
                name: "App",
                operation: service => service.GetByDomainApp(
                    domain: domain,
                    ignoreFilters: ignoreFilters));

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            IQueryable<App>>(
                name: "App",
                operation: service => service.GetAllApp(
                    ignoreFilters: ignoreFilters));

    public ValueTask<App> AddAsync(App newApp) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            ValueTask<App>>(
                name: "App",
                operation: service => service.AddAppAsync(
                    newApp: newApp));

    public ValueTask<App> UpdateAsync(App updatedApp) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            ValueTask<App>>(
                name: "App",
                operation: service => service.UpdateAppAsync(
                    updatedApp: updatedApp));

    public ValueTask DeleteAsync(int appId) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            ValueTask>(
                name: "App",
                operation: service => service.DeleteAsync(
                    appId: appId));

    public bool IsAdmin(int appId, string userName) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            bool>(
                name: "App",
                operation: service => service.IsAdminApp(
                    appId: appId,
                    userName: userName));

    public IQueryable<User> GetUsers(int appId) =>
        serviceProviderExecutionService.Execute<
            IAppUserProcessingService,
            IQueryable<User>>(
                name: "AppUser",
                operation: service => service.GetAppUsers(
                    appId: appId));

    public ValueTask UpdatePageOrderAsync(int appId, App updatedApp) =>
        serviceProviderExecutionService.Execute<
            IAppOrchestrationService,
            ValueTask>(
                name: "App",
                operation: service => service.UpdatePageOrderAppAsync(
                    key: appId,
                    updatedApp: updatedApp));
}