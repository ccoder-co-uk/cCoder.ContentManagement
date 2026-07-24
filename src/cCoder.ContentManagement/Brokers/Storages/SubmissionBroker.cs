// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.ContentManagement.Brokers.Storages;

public class SubmissionBroker(ICoreContextFactory coreContextFactory) : ISubmissionBroker
{
    public IQueryable<Submission> GetAllSubmissions(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Submissions.IgnoreQueryFilters()
            : coreDataContext.Submissions;
    }

    public async ValueTask<Submission> AddSubmissionAsync(Submission entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Submission result = (await coreDataContext.Submissions.AddAsync(entity: entity)).Entity;
        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Submission> UpdateSubmissionAsync(Submission entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Submission result = coreDataContext.Submissions.Update(entity: entity)
            .Entity;

        await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteSubmissionAsync(Submission entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Submissions.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllSubmissionsAsync(IEnumerable<Submission> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Submissions.RemoveRange(entities: items);
        await coreDataContext.SaveChangesAsync();
    }

}