// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class SubmissionEventService(ISubmissionEventBroker submissionEventBroker, ICoreAuthInfo authInfo) : ISubmissionEventService
{
    public async ValueTask RaiseSubmissionAddEventAsync(Submission entity)
    {
        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionAddEventAsync(message: message);
    }

    public async ValueTask RaiseSubmissionUpdateEventAsync(Submission entity)
    {
        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseSubmissionDeleteEventAsync(Submission entity)
    {
        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionDeleteEventAsync(message: message);
    }
}