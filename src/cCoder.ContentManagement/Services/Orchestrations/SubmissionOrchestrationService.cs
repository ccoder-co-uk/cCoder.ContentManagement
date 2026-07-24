// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class SubmissionOrchestrationService(
    ISubmissionProcessingService processingService,
    ISubmissionEventProcessingService eventService) : ISubmissionOrchestrationService
{
    public Submission Get(Guid id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<Submission> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Submission> AddAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");

        Submission result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseSubmissionAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Submission> UpdateAsync(Submission entity)
    {
        ValidateSubmission(submission: entity, parameterName: "entity");

        Submission result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseSubmissionUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        ValidateId(id: id, parameterName: "id");

        Submission entity = processingService.Get(id: id);
        await eventService.RaiseSubmissionDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Submission>>> AddOrUpdate(IEnumerable<Submission> items) =>
        processingService.AddOrUpdate(items: ValidateSubmissions(submissions: items, parameterName: "items"));

    public ValueTask DeleteAllAsync(IEnumerable<Submission> items) =>
        processingService.DeleteAllAsync(items: ValidateSubmissions(submissions: items, parameterName: "items"));

    private static Guid ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return id;
    }

    private static Submission ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return submission;
    }

    private static IEnumerable<Submission> ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName)
    {
        if (submissions == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return submissions;
    }
}