// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class ComponentManager(
    IComponentOrchestrationService componentOrchestrationService)
        : IComponentManager
{
    public IQueryable<Component> GetAll() =>
        componentOrchestrationService.GetAllComponent();

    public Component Get(int componentId) =>
        componentOrchestrationService.GetComponent(componentId: componentId);

    public ValueTask<Component> AddAsync(Component newComponent) =>
        componentOrchestrationService.AddComponentAsync(newComponent: newComponent);

    public ValueTask<Component> UpdateAsync(Component updatedComponent) =>
        componentOrchestrationService.UpdateComponentAsync(updatedComponent: updatedComponent);

    public ValueTask DeleteAsync(int componentId) =>
        componentOrchestrationService.DeleteAsync(componentId: componentId);

    public ValueTask ImportComponentsAsync(int appId, Component[] items) =>
        componentOrchestrationService.ImportComponentsAsync(
            appId: appId,
            items: items);
}