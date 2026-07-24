// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementDependencyException(
    Exception innerException)
    : InvalidOperationException(
        message: innerException.Message,
        innerException: innerException)
{
}