// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementValidationException(
    Exception innerException)
    : System.ComponentModel.DataAnnotations.ValidationException(
        message: innerException.Message,
        innerException: innerException)
{
}