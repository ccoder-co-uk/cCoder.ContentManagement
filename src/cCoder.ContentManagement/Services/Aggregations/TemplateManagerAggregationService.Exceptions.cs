// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class TemplateManagerAggregationService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try { return operation(); }
        catch (ContentManagementValidationException innerException) { throw new ContentManagementValidationException(innerException: innerException); }
        catch (ContentManagementDependencyException innerException) { throw new ContentManagementDependencyException(innerException: innerException); }
        catch (Exception innerException) { throw new ContentManagementServiceException(innerException: innerException); }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation, bool isValueTask)
    {
        try { await operation(); }
        catch (ContentManagementValidationException innerException) { throw new ContentManagementValidationException(innerException: innerException); }
        catch (ContentManagementDependencyException innerException) { throw new ContentManagementDependencyException(innerException: innerException); }
        catch (Exception innerException) { throw new ContentManagementServiceException(innerException: innerException); }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation, bool isValueTask)
    {
        try { return await operation(); }
        catch (ContentManagementValidationException innerException) { throw new ContentManagementValidationException(innerException: innerException); }
        catch (ContentManagementDependencyException innerException) { throw new ContentManagementDependencyException(innerException: innerException); }
        catch (Exception innerException) { throw new ContentManagementServiceException(innerException: innerException); }
    }
}