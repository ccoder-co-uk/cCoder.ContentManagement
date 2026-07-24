// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionProcessingService(ISubmissionService service) : ISubmissionProcessingService
{
    public Submission Get(Guid id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.Get(id: id);
    }

    public IQueryable<Submission> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Submission> AddAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");
        return service.AddAsync(submission: entity);
    }

    public ValueTask<Submission> UpdateAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");
        return service.UpdateAsync(submission: entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id: id, parameterName: "id");
        return service.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<Submission>>> AddOrUpdate(IEnumerable<Submission> items)
    {
        ValidateSubmissions(submissions: items, parameterName: "items");
        List<Result<Submission>> results = new List<Result<Submission>>();

        foreach (Submission item in items)
        {
            try
            {
                Submission savedItem = item.Id == Guid.Empty ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

                results.Add(item: new Result<Submission>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Submission>
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
        ValidateSubmissions(submissions: items, parameterName: "items");

        foreach (Submission item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }

    private static void ValidateId(Guid id, string parameterName) =>
        ThrowIf(condition: id == Guid.Empty, message: parameterName + " is required.");

    private static void ValidateSubmission(Submission submission, string parameterName) =>
        ThrowIf(condition: submission == null, message: parameterName + " is required.");

    private static void ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName) =>
        ThrowIf(condition: submissions == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}