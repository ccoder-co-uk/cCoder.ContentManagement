// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class CurrentAppProcessingService
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
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(
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
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(
                innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(
                innerException: innerException);
        }
    }
}