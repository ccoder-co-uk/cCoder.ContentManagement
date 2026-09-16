// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheSourceService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try { return operation(); }
        catch (ContentManagementValidationException exception) { throw new ContentManagementValidationException(innerException: exception); }
        catch (ContentManagementDependencyException exception) { throw new ContentManagementDependencyException(innerException: exception); }
        catch (Exception exception) { throw new ContentManagementServiceException(innerException: exception); }
    }
}