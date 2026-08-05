// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations;

internal sealed partial class ContentManagementMetadataTypeService
{
    private static void ValidateKnownMetadataOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}