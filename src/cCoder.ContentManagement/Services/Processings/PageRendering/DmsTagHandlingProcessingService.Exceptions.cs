// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class DmsTagHandlingProcessingService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ContentManagementValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (ContentManagementDependencyException innerException)
        {
            throw new ContentManagementDependencyException(innerException);
        }
        catch (System.ComponentModel.DataAnnotations.ValidationException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new ContentManagementValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ContentManagementDependencyException(innerException);
        }
        catch (Exception innerException)
        {
            throw new ContentManagementServiceException(innerException);
        }
    }
}