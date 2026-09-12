// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class PagePackageImportCoordinationService
{
    private static void ValidateImportPagesAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}