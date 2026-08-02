// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheQueryProcessingService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try { return operation(); }
        catch (ContentManagementValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ContentManagementDependencyException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (ArgumentException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (InvalidOperationException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (System.Security.SecurityException exception) { throw new ContentManagementSecurityException(innerException: exception); }
        catch (TaskCanceledException exception) { throw new ContentManagementTaskCanceledException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }
}