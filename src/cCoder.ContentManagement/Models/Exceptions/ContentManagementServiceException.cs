// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementServiceException(
    Exception innerException)
    : Exception(
        message: "A content management service failed.",
        innerException: innerException)
{
}