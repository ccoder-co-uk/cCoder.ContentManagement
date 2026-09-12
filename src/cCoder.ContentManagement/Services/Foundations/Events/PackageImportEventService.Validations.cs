// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal sealed partial class PackageImportEventService
{
    private static void ValidateRaiseImportAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}