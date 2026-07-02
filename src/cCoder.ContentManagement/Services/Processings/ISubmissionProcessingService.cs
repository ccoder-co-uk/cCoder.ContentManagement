using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

public interface ISubmissionProcessingService
{
    Submission Get(Guid id);

    IQueryable<Submission> GetAll(bool ignoreFilters = false);

    ValueTask<Submission> AddAsync(Submission entity);

    ValueTask<Submission> UpdateAsync(Submission entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<Submission>>> AddOrUpdate(IEnumerable<Submission> items);

    ValueTask DeleteAllAsync(IEnumerable<Submission> items);
}
