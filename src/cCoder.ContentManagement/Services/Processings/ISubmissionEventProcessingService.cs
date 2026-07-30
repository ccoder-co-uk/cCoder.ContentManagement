// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ISubmissionEventProcessingService
{
    ValueTask RaiseSubmissionAddEventAsync(Submission entity);

    ValueTask RaiseSubmissionUpdateEventAsync(Submission entity);

    ValueTask RaiseSubmissionDeleteEventAsync(Submission entity);
}