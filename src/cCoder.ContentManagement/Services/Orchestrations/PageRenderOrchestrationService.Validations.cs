// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRenderOrchestrationService
{
    private static void ValidateProcessPageRenderOperation(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateIsAdminOfApp(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResolveCulture(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUserCanPage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderPageRenderResult(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateRenderPageUserRenderResult(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}