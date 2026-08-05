// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class UncachedPageRenderOrchestrationService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation,
        bool isValueTask)
    {
        try
        {
            return await operation();
        }
        catch (PageNotFoundException)
        {
            throw;
        }
        catch (PageAccessSecurityException)
        {
            throw;
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