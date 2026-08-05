// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal sealed partial class PageRenderCacheService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try { return operation(); }
        catch (System.ComponentModel.DataAnnotations.ValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ArgumentException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (InvalidOperationException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (TaskCanceledException exception) { throw new ContentManagementTaskCanceledException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation, bool isValueTask)
    {
        try { await operation(); }
        catch (System.ComponentModel.DataAnnotations.ValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ArgumentException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (InvalidOperationException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (TaskCanceledException exception) { throw new ContentManagementTaskCanceledException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation, bool isValueTask)
    {
        try { return await operation(); }
        catch (System.ComponentModel.DataAnnotations.ValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ArgumentException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (InvalidOperationException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (TaskCanceledException exception) { throw new ContentManagementTaskCanceledException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }
}