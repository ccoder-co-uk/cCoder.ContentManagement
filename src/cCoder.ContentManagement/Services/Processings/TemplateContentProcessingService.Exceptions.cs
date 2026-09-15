// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateContentProcessingService
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