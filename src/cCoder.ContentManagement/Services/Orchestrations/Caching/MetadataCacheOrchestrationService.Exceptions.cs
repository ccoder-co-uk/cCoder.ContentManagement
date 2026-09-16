// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal sealed partial class MetadataCacheOrchestrationService
{
    private static void TryCatch(Action operation)
    {
        try { operation(); }
        catch (ContentManagementValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ContentManagementDependencyException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try { return operation(); }
        catch (ContentManagementValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ContentManagementDependencyException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }
}