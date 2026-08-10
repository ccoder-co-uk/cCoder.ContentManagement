// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Packaging;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Web.Controllers;

[ApiController]
[Route("Api/ContentManagement/Package")]
public sealed class PackageController(
    IContentManagementPackageManager contentManagementPackageManager,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost("Import")]
    public async Task<IActionResult> PostImportAsync([FromQuery] int appId, [FromBody] Package newPackage)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await contentManagementPackageManager.ImportPackageAsync(
                appId: appId,
                package: newPackage);

            return Ok();
        }
        catch (ContentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Package import validation failed.");

            return BadRequest();
        }
        catch (ContentManagementSecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Package import authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Package import failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}