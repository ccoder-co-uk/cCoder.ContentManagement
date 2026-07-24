// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ICultureService
{
    Culture GetCulture(string cultureId, bool ignoreFilters = false);

    IQueryable<Culture> GetAllCulture(bool ignoreFilters = false);

    ValueTask<Culture> AddCultureAsync(Culture newCulture);

    ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture);

    ValueTask DeleteAsync(string cultureId);
}