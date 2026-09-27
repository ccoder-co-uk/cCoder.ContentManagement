// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            return File(
                fileContents: content,
                contentType: "application/pdf",
                fileDownloadName: name + ".pdf");
        }
        catch (ContentManagementValidationException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return BadRequest();
        }
        catch (ContentManagementSecurityException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}