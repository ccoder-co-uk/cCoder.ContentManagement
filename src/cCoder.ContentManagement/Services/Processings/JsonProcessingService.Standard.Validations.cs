// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class JsonProcessingService
{
    private static void ValidateDeserializeItems(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}