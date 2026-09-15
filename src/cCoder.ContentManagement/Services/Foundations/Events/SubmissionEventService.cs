// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.Data;
using cCoder.Eventing.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class SubmissionEventService(ISubmissionEventBroker submissionEventBroker) : ISubmissionEventService
{
    public ValueTask RaiseSubmissionAddEventAsync(Submission submission, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [submission, userId]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission submission, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [submission, userId]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission submission, string userId) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [submission, userId]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = userId
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionDeleteEventAsync(message: message);

    }, isValueTask: true);
}