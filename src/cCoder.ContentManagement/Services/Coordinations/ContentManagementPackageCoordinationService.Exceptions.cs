// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class ContentManagementPackageCoordinationService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (ContentManagementSecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (ContentManagementTaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(
        Func<ValueTask> operation,
        bool isValueTask)
    {
        try
        {
            await operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (ContentManagementSecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (ContentManagementTaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }
}