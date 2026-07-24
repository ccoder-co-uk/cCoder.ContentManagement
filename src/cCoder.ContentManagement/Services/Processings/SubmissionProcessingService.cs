// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class SubmissionProcessingService(ISubmissionService service) : ISubmissionProcessingService
{
    public Submission GetSubmission(Guid submissionId) =>
        TryCatch<Submission>(operation: () =>
    {
        ValidateSubmissionOnGet(inputs: [submissionId]);
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.GetSubmission(submissionId: submissionId);

    });

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Submission>>(operation: () =>
    {
        ValidateAllSubmissionOnGet(inputs: [ignoreFilters]);
        return service.GetAllSubmission(ignoreFilters: ignoreFilters);
    });

    public ValueTask<Submission> AddSubmissionAsync(Submission newSubmission) =>
        TryCatch<Submission>(operation: () =>
    {
        ValidateSubmissionOnAdd(inputs: [newSubmission]);
        ValidateSubmission(submission: newSubmission, parameterName: "entity");
        return service.AddSubmissionAsync(newSubmission: newSubmission);

    }, isValueTask: true);

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission) =>
        TryCatch<Submission>(operation: () =>
    {
        ValidateSubmissionOnUpdate(inputs: [updatedSubmission]);
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");
        return service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid submissionId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [submissionId]);
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.DeleteAsync(submissionId: submissionId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission) =>
        TryCatch<IEnumerable<OperationResult<Submission>>>(operation: async () =>
    {
        ValidateOrUpdateSubmissionResultOnAdd(inputs: [newSubmission]);
        ValidateSubmissions(submissions: newSubmission, parameterName: "items");
        List<OperationResult<Submission>> results = new List<OperationResult<Submission>>();

        foreach (Submission item in newSubmission)
        {
            try
            {
                Submission savedItem = item.Id == Guid.Empty ? await ExecuteAddSubmissionAsync(newSubmission: item) : await ExecuteUpdateSubmissionAsync(updatedSubmission: item);

                results.Add(item: new OperationResult<Submission>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<Submission>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission) =>
        TryCatch(operation: async () =>
    {
        ValidateAllSubmissionOnDelete(inputs: [deletedSubmission]);
        ValidateSubmissions(submissions: deletedSubmission, parameterName: "items");

        foreach (Submission item in deletedSubmission)
        {
            await ExecuteDeleteAsync(submissionId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateId(Guid submissionId, string parameterName) =>
        ThrowIf(condition: submissionId == Guid.Empty, message: parameterName + " is required.");

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

    private ValueTask<Submission> ExecuteAddSubmissionAsync(Submission newSubmission)
    {
        ValidateSubmission(submission: newSubmission, parameterName: "entity");
        return service.AddSubmissionAsync(newSubmission: newSubmission);
    }

    private ValueTask ExecuteDeleteAsync(Guid submissionId)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.DeleteAsync(submissionId: submissionId);
    }

    private ValueTask<Submission> ExecuteUpdateSubmissionAsync(Submission updatedSubmission)
    {
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");
        return service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);
    }
}