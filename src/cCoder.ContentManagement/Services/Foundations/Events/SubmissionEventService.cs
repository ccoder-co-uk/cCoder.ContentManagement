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
    public ValueTask RaiseSubmissionAddEventAsync(Submission submission) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionAddEventAsync(inputs: [submission]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = submissionEventBroker.GetCurrentUserId()
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionAddEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionUpdateEventAsync(Submission submission) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionUpdateEventAsync(inputs: [submission]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = submissionEventBroker.GetCurrentUserId()
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionUpdateEventAsync(message: message);

    }, isValueTask: true);

    public ValueTask RaiseSubmissionDeleteEventAsync(Submission submission) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseSubmissionDeleteEventAsync(inputs: [submission]);

        EventMessage<Submission> message = new EventMessage<Submission>
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = submissionEventBroker.GetCurrentUserId()
            },
            Data = submission
        };

        await submissionEventBroker.RaiseSubmissionDeleteEventAsync(message: message);

    }, isValueTask: true);
}