// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface IComponentService
{
    Component Get(int id, bool ignoreFilters = false);

    IQueryable<Component> GetAll(bool ignoreFilters = false);

    ValueTask<Component> AddAsync(Component component);

    ValueTask<Component> UpdateAsync(Component component);

    ValueTask DeleteAsync(int id);
}