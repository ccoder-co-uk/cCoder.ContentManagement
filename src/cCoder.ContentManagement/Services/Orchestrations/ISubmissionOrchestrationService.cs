using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ISubmissionOrchestrationService
{
    Submission Get(Guid id);

    IQueryable<Submission> GetAll(bool ignoreFilters = false);

    ValueTask<Submission> AddAsync(Submission entity);

    ValueTask<Submission> UpdateAsync(Submission entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<Submission>>> AddOrUpdate(IEnumerable<Submission> items);

    ValueTask DeleteAllAsync(IEnumerable<Submission> items);
}
