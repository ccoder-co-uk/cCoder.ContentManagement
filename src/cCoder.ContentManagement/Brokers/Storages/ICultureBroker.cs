// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ICultureBroker
{
    IQueryable<Culture> GetAllCultures(bool ignoreFilters);

    ValueTask<Culture> AddCultureAsync(Culture newCulture);

    ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture);

    ValueTask<int> DeleteCultureAsync(Culture deletedCulture);

    ValueTask DeleteAllCulturesAsync(IEnumerable<Culture> deletedCulture);
}