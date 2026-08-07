// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IComponentManager
{
    IQueryable<Component> GetAll();

    Component Get(int componentId);

    ValueTask<Component> AddAsync(Component newComponent);

    ValueTask<Component> UpdateAsync(Component updatedComponent);

    ValueTask DeleteAsync(int componentId);

    ValueTask ImportComponentsAsync(int appId, Component[] items);
}