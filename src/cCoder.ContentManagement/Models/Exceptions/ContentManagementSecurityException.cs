// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementSecurityException(
    Exception innerException)
    : SecurityException(
        message: innerException.Message,
        inner: innerException)
{
}