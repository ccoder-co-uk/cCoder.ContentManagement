using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionProcessingService(ISubmissionService service) : ISubmissionProcessingService
{
    public Submission Get(Guid id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<Submission> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<Submission> AddAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<Submission> UpdateAsync(Submission entity)
    {
        ValidateSubmission(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<Submission>>> AddOrUpdate(IEnumerable<Submission> items)
    {
        ValidateSubmissions(items, "items");
        List<Result<Submission>> results = new List<Result<Submission>>();
        foreach (Submission item in items)
        {
            try
            {
                Submission savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<Submission>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Submission>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Submission> items)
    {
        ValidateSubmissions(items, "items");
        foreach (Submission item in items)
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(Guid id, string parameterName) =>
        ThrowIf(id == Guid.Empty, parameterName + " is required.");

    private static void ValidateSubmission(Submission submission, string parameterName) =>
        ThrowIf(submission == null, parameterName + " is required.");

    private static void ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName) =>
        ThrowIf(submissions == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
