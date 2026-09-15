// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ContentManagementPackageCoordinationService
{
    private static void ValidateImportPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateExportPackage(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}