// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class HttpPageRenderOperationEventService
{
    private static void ValidateRaiseHttpPageRenderOperationRenderRequestAsync(
        object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}