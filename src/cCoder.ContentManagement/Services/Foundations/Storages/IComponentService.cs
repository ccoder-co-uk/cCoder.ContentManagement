// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface IComponentService
{
    Component GetComponent(int componentId, bool ignoreFilters = false);

    IQueryable<Component> GetAllComponents(bool ignoreFilters = false);

    ValueTask<Component> AddComponentAsync(Component newComponent);

    ValueTask<Component> UpdateComponentAsync(Component updatedComponent);

    ValueTask DeleteAsync(int componentId);
}