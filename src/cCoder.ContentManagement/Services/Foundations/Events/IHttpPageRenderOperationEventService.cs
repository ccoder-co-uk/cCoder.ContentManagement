// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IHttpPageRenderOperationEventService
{
    ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        HttpPageRenderOperation httpPageRenderOperation,
        string userId);
}