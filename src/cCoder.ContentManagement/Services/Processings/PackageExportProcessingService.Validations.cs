// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageExportProcessingService
{
    private static void ValidateExportPackage(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}