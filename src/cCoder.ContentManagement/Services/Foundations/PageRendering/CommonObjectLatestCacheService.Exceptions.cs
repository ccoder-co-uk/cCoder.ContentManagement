// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectLatestCacheService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(
                innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(
                innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(
                innerException: innerException);
        }
    }

    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(
                innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(
                innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(
                innerException: innerException);
        }
    }
}