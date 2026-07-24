// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IComponentOrchestrationService
{
    Component GetComponent(int componentId);

    IQueryable<Component> GetAllComponent(bool ignoreFilters = false);

    ValueTask<Component> AddComponentAsync(Component newComponent);

    ValueTask<Component> UpdateComponentAsync(Component updatedComponent);

    ValueTask DeleteAsync(int componentId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Component>>> AddOrUpdateComponentResult(IEnumerable<Component> newComponent);

    ValueTask ImportComponentsAsync(int appId, Component[] items);

    ValueTask DeleteAllComponentAsync(IEnumerable<Component> deletedComponent);
}