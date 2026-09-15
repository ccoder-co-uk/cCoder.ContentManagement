// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ISubmissionEventService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity, string userId);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity, string userId);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity, string userId);
}