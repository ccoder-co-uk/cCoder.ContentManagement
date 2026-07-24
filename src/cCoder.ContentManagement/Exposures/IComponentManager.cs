// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

public interface IComponentManager
{
    string Render(int appId, string name, string culture, string theme);

    IQueryable<Component> GetAll();

    Component Get(int componentId);

    ValueTask<Component> AddAsync(Component newComponent);

    ValueTask<Component> UpdateAsync(Component updatedComponent);

    ValueTask DeleteAsync(int componentId);
}