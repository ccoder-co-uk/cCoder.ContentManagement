// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ISubmissionEventProcessingService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity, string userId);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity, string userId);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity, string userId);
}