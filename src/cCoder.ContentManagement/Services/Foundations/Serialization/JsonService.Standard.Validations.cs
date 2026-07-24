// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService
{
    private static void ValidateDeserialize(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}