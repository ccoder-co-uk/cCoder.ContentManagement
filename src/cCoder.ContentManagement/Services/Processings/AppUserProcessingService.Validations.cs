// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class AppUserProcessingService
{
    private static void ValidateAppUsersOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}