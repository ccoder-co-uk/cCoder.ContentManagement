// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementValidationException(
    Exception innerException)
    : Exception(
        message: "Content management validation failed.",
        innerException: innerException)
{
}