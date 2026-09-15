// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class AppManagerAggregationService
{
    private static void ValidateAppManagerContextOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByDomainAppManagerContextOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllAppManagerContextOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppManagerContextOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppManagerContextOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppManagerContextOnDelete(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAdminAppManagerContextOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateUsersAppManagerContextOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageOrderAppManagerContextOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}