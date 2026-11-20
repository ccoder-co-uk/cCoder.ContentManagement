// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Authentication;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
public sealed class TemplateContentController(
    ITemplateContentOrchestrationService service,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost("Api/ContentManagement/Template/HtmlToPdf")]
    [AllowAnonymous]
    public async Task<IActionResult> PostHtmlToPdfAsync(string name)
    {
        try
        {
            string htmlContent = await service.ReadContentAsync(
                source: base.Request.Body);

            byte[] content = service.ConvertHtmlToPdf(html: htmlContent);


            {

                loggingBroker.LogInformation(message: "Controller action completed successfully.");

                return File(fileContents: content, contentType: "application/pdf", fileDownloadName: name + ".pdf");

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