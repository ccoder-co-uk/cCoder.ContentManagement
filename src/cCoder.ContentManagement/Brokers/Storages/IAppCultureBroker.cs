// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface IAppCultureBroker
{
    IQueryable<AppCulture> GetAllAppCultures();

    IQueryable<AppCulture> GetAllAppCulturesIgnoringFilters();

    ValueTask<AppCulture> AddAppCultureAsync(AppCulture newAppCulture);

    ValueTask<int> DeleteAppCultureAsync(AppCulture deletedAppCulture);

    ValueTask DeleteAllAppCulturesAsync(IEnumerable<AppCulture> deletedAppCulture);
}