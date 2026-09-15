// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cCoder.ContentManagement.Api.OData;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(modelState)
    {
        ModelStateError[] errors = modelState
            .Select(
                selector: (KeyValuePair<string, ModelStateEntry> item) =>
                    new ModelStateError
                    {
                        Key = item.Key,
                        Value = item.Value?.RawValue,
                        Errors = item.Value?.Errors?
                            .Select(
                                selector: (ModelError error) =>
                                    error.ErrorMessage + " - " + error.Exception?.Message)
                            .ToArray()
                    })
            .ToArray();

        base.Value = errors;
    }
}