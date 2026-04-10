using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Brokers.Storages;

public interface ISubmissionBroker
{
    IQueryable<Submission> GetAllSubmissions(bool ignoreFilters);

    ValueTask<Submission> AddSubmissionAsync(Submission entity);

    ValueTask<Submission> UpdateSubmissionAsync(Submission entity);

    ValueTask<int> DeleteSubmissionAsync(Submission entity);

    ValueTask DeleteAllSubmissionsAsync(IEnumerable<Submission> items);
}
