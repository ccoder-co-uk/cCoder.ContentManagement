// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
namespace cCoder.ContentManagement.Models.Exceptions;

public sealed class ContentManagementTaskCanceledException(
    TaskCanceledException innerException)
    : TaskCanceledException(
        message: innerException.Message,
        innerException: innerException,
        token: innerException.CancellationToken)
{
}