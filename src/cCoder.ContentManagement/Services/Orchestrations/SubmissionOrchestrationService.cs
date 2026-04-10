using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using Submission = cCoder.Data.Models.CMS.Submission;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.Submission>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class SubmissionOrchestrationService(
    ISubmissionProcessingService processingService,
    ISubmissionEventProcessingService eventService) : ISubmissionOrchestrationService
{
    public Submission Get(Guid id) => processingService.Get(ValidateId(id, "id"));

    public IQueryable<Submission> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Submission> AddAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");

        Submission result = await processingService.AddAsync(entity);
        await eventService.RaiseSubmissionAddEventAsync(result);
        return result;
    }

    public async ValueTask<Submission> UpdateAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");

        Submission result = await processingService.UpdateAsync(entity);
        await eventService.RaiseSubmissionUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id, "id");

        Submission entity = processingService.Get(id);
        await eventService.RaiseSubmissionDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<Submission> items) =>
        processingService.AddOrUpdate(ValidateSubmissions(items, "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Submission> items) =>
        processingService.DeleteAllAsync(ValidateSubmissions(items, "items"));

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return id;
    }

    private static Submission ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return submission;
    }

    private static IEnumerable<Submission> ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName)
    {
        if (submissions == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }

        return submissions;
    }
}
