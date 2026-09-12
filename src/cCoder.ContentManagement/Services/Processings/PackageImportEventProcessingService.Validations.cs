// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PackageImportEventProcessingService
{
    private static void ValidateRaiseImportAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}