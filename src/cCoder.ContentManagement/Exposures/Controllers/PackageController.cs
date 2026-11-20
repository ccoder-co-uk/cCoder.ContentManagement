// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Packaging;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Authentication;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
[Route("Api/ContentManagement/Package")]
public sealed class PackageController(
    IContentManagementPackageImportOrchestrationService packageImportOrchestrationService,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost("Import")]
    public async Task<IActionResult> PostImportAsync(
        [FromQuery] int? appId,
        [FromBody] Package newPackage)
    {
        try
        {
            if (!ModelState.IsValid)
            {

                {
                    loggingBroker.LogWarning(message: "Controller action returned a client error.");
                    return BadRequest(modelState: ModelState);
                }
            }

            await packageImportOrchestrationService.ImportPackageAsync(
                appId: appId,
                package: newPackage);


            {

                loggingBroker.LogInformation(message: "Controller action completed successfully.");

                return Ok();

            }
        }

        catch (ContentManagementValidationException exception)
{
    loggingBroker.LogWarning(message: "Controller validation failed.", args: exception);
    return BadRequest(error: "The content management request is invalid.");
}

        catch (ArgumentException exception)
{
    loggingBroker.LogWarning(message: "Controller argument validation failed.", args: exception);
    return BadRequest(error: "The content management request is invalid.");
}

        catch (AuthenticationException exception)
{
    loggingBroker.LogWarning(message: "Controller authentication failed.", args: exception);
    return StatusCode(statusCode: StatusCodes.Status401Unauthorized, value: "Authentication failed.");
}

        catch (UnauthorizedAccessException exception)
{
    loggingBroker.LogWarning(message: "Controller authorization failed.", args: exception);
    return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "The content management operation is forbidden.");
}

        catch (ContentManagementSecurityException exception)
{
    loggingBroker.LogWarning(message: "Controller security check failed.", args: exception);
    return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "The content management operation is forbidden.");
}

        catch (SecurityException exception)
{
    loggingBroker.LogWarning(message: "Controller security check failed.", args: exception);
    return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "The content management operation is forbidden.");
}

        catch (Exception exception)
{
    loggingBroker.LogError(exception: exception, message: "Controller request failed.");
    return StatusCode(statusCode: StatusCodes.Status500InternalServerError, value: "The content management operation failed.");
}
        }
}