// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.ServiceProviders;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ComponentManager(
    IServiceProviderExecutionService serviceProviderExecutionService)
        : IComponentManager
{
    public IQueryable<Component> GetAll() =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            IQueryable<Component>>(
                name: "Component",
                operation: service => service.GetAllComponent());

    public Component Get(int componentId) =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            Component>(
                name: "Component",
                operation: service => service.GetComponent(
                    componentId: componentId));

    public ValueTask<Component> AddAsync(Component newComponent) =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            ValueTask<Component>>(
                name: "Component",
                operation: service => service.AddComponentAsync(
                    newComponent: newComponent));

    public ValueTask<Component> UpdateAsync(Component updatedComponent) =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            ValueTask<Component>>(
                name: "Component",
                operation: service => service.UpdateComponentAsync(
                    updatedComponent: updatedComponent));

    public ValueTask DeleteAsync(int componentId) =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            ValueTask>(
                name: "Component",
                operation: service => service.DeleteAsync(
                    componentId: componentId));

    public ValueTask ImportComponentsAsync(int appId, Component[] items) =>
        serviceProviderExecutionService.Execute<
            IComponentOrchestrationService,
            ValueTask>(
                name: "Component",
                operation: service => service.ImportComponentsAsync(
                    appId: appId,
                    items: items));
}