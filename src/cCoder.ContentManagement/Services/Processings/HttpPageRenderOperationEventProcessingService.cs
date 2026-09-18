// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class HttpPageRenderOperationEventProcessingService(
    IHttpPageRenderOperationEventService eventService)
        : IHttpPageRenderOperationEventProcessingService
{
    public ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        HttpPageRenderOperation httpPageRenderOperation,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseHttpPageRenderOperationRenderRequestAsync(
            inputs: [httpPageRenderOperation, userId]);

        ThrowIf(
            condition: httpPageRenderOperation is null,
            message: "The HTTP page render operation is required.");

        return eventService.RaiseHttpPageRenderOperationRenderRequestAsync(
            httpPageRenderOperation: httpPageRenderOperation,
            userId: userId);
    }, isValueTask: true);

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}