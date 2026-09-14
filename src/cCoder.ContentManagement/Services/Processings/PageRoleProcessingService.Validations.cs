// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageRoleProcessingService
{
    private static void ValidateAllPageRoleOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageRoleOnResolve(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdatePageRoleResultOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllPageRoleOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}