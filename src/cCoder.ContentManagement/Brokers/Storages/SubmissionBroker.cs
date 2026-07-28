// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

internal sealed class SubmissionBroker(ICoreContextFactory coreContextFactory) : ISubmissionBroker
{
    public IQueryable<Submission> GetAllSubmissions(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Extensions.Data.QueryFilterExtensions.Apply(
            query: coreDataContext.Submissions,
            ignoreFilters: ignoreFilters);
    }

    public async ValueTask<Submission> AddSubmissionAsync(Submission newSubmission)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Submission result = (await coreDataContext.Submissions.AddAsync(entity: newSubmission)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Submission result = coreDataContext.Submissions.Update(entity: updatedSubmission)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteSubmissionAsync(Submission deletedSubmission)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Submissions.Remove(entity: deletedSubmission);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllSubmissionsAsync(IEnumerable<Submission> deletedSubmission)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Submissions.RemoveRange(entities: deletedSubmission);
        await coreDataContext.SaveChangesAsync();
    }

}