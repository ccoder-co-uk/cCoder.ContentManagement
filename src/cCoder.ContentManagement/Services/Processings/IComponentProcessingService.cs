// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface IComponentProcessingService
{
    Component GetComponent(int componentId);

    IQueryable<Component> GetAllComponent(bool ignoreFilters = false);

    ValueTask<Component> AddComponentAsync(Component newComponent);

    ValueTask<Component> UpdateComponentAsync(Component updatedComponent);

    ValueTask DeleteAsync(int componentId);

    ValueTask<IEnumerable<Result<Component>>> AddOrUpdateComponentResult(IEnumerable<Component> newComponent);

    ValueTask DeleteAllComponentAsync(IEnumerable<Component> deletedComponent);
}