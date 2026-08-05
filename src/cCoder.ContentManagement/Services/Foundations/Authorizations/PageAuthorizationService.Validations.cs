// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Authorizations;

internal sealed partial class PageAuthorizationService
{
    private static void ValidateAuthorizeHttpPageRenderContextAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderContext(
        HttpPageRenderContext pageRenderContext,
        string parameterName)
    {
        if (pageRenderContext is null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }
    }
}