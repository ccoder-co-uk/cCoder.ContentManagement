// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal interface ICultureService
{
    Culture GetCulture(string cultureId, bool ignoreFilters = false);

    IQueryable<Culture> GetAllCulture(bool ignoreFilters = false);

    int? GetOwningAppId(string cultureId);

    ValueTask<Culture> AddCultureAsync(Culture newCulture);

    ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture);

    ValueTask DeleteAsync(string cultureId);
}