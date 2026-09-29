// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementDependencyException(
    Exception innerException)
    : InvalidOperationException(
        message: innerException.Message,
        innerException: innerException)
{
}