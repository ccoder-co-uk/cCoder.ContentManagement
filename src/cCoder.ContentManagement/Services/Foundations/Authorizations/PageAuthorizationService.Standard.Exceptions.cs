// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Foundations.Authorizations;

internal sealed partial class PageAuthorizationService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation,
        bool isValueTask)
    {
        try
        {
            return await operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException);
        }
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException);
        }
    }
}