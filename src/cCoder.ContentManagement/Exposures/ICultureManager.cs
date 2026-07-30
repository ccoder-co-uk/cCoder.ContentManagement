// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Exposures;

public interface ICultureManager
{
    Culture GetCulture(string cultureId);

    IQueryable<Culture> GetAllCulture(bool ignoreFilters = false);

    ValueTask<Culture> AddCultureAsync(Culture newCulture);

    ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture);

    ValueTask DeleteAsync(string cultureId);

    ValueTask<IEnumerable<OperationResult<Culture>>> AddOrUpdateCultureResult(IEnumerable<Culture> newCulture);

    ValueTask DeleteAllCultureAsync(IEnumerable<Culture> deletedCulture);
}