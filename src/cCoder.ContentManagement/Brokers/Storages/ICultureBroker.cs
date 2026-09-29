// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ICultureBroker
{
    IQueryable<Culture> GetAllCultures();

    IQueryable<Culture> GetAllCulturesIgnoringFilters();

    int? GetOwningAppId(string cultureId);

    ValueTask<Culture> AddCultureAsync(Culture newCulture);

    ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture);

    ValueTask<int> DeleteCultureAsync(Culture deletedCulture);

    ValueTask DeleteAllCulturesAsync(IEnumerable<Culture> deletedCulture);
}