// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IComponentBroker
{
    IQueryable<Component> GetAllComponents();

    IQueryable<Component> GetAllComponentsIgnoringFilters();

    ValueTask<Component> AddComponentAsync(Component newComponent);

    ValueTask<Component> UpdateComponentAsync(Component updatedComponent);

    ValueTask<int> DeleteComponentAsync(Component deletedComponent);

    ValueTask DeleteAllComponentsAsync(IEnumerable<Component> deletedComponent);
}