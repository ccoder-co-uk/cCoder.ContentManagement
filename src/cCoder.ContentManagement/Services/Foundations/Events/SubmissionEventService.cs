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
    public ValueTask RaiseSubmissionAddEventAsync(Submission entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [entity]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [entity]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission entity) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [entity]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = authInfo.SSOUserId
            },
            Data = entity
        };

        await submissionEventBroker.RaiseSubmissionDeleteEventAsync(message: message);

    }, isValueTask: true);
}