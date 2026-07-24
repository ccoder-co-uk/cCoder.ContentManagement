// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementDependencyException(
    Exception innerException)
    : Exception(
        message: "A content management dependency failed.",
        innerException: innerException)
{
}