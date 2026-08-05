// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectCacheProcessingService
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
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException: innerException);
        }
    }
}