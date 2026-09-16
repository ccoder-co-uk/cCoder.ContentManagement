// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures;

internal sealed class CultureManager(ICultureOrchestrationService service) : ICultureManager
{
    public Culture GetCulture(string cultureId) =>
        service.GetCulture(cultureId: cultureId);

    public IQueryable<Culture> GetAllCulture(bool ignoreFilters = false) =>
        service.GetAllCulture(ignoreFilters: ignoreFilters);

    public ValueTask<Culture> AddCultureAsync(Culture newCulture) =>
        service.AddCultureAsync(newCulture: newCulture);

    public ValueTask<Culture> UpdateCultureAsync(Culture updatedCulture) =>
        service.UpdateCultureAsync(updatedCulture: updatedCulture);

    public ValueTask DeleteAsync(string cultureId) =>
        service.DeleteAsync(cultureId: cultureId);

    public ValueTask<IEnumerable<OperationResult<Culture>>> AddOrUpdateCultureResult(
        IEnumerable<Culture> newCulture) =>
        service.AddOrUpdateCultureResult(newCulture: newCulture);

    public ValueTask DeleteAllCultureAsync(IEnumerable<Culture> deletedCulture) =>
        service.DeleteAllCultureAsync(deletedCulture: deletedCulture);
}