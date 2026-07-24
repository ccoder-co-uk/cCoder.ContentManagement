// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateRenderProcessingService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }

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
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }

    private static async Task TryCatch(
        Func<Task> operation,
        bool isTask)
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
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }

    private static async Task<TResult> TryCatch<TResult>(
        Func<Task<TResult>> operation,
        bool isTask)
    {
        try
        {
            return await operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
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
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }

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
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException: innerException);
        }
        catch (System.Security.SecurityException innerException)
        {
            throw new ContentManagementSecurityException(innerException: innerException);
        }
        catch (TaskCanceledException innerException)
        {
            throw new ContentManagementTaskCanceledException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }
}