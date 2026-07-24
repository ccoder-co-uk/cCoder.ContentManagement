// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRenderOrchestrationService
{
    private static void ValidateIsAdminOfApp(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateResolveCulture(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateUserCanPage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderPageRenderResult(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateRenderPageUserRenderResult(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}