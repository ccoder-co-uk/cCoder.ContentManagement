// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
[Route(template: "Api/ContentManagement")]
public sealed class RenderController(
    IRenderer renderer,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost(template: "Template/Render()")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> PostTemplate(
        string name,
        [FromBody] object model)
    {
        try
        {
            TemplateRenderResult result = (TemplateRenderResult)
                await renderer.RenderTemplateRenderResultAsync(
                name: name,
                model: model);

            return Content(content: result.Content, contentType: "text/plain");
        }
        catch (ContentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest();
        }
        catch (ContentManagementSecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(template: "Component/Render()")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetComponent(string name)
    {
        try
        {
            ComponentRenderResult result = (ComponentRenderResult)
                await renderer
                    .RenderComponentRenderResultAsync(name: name);

            return result.Content is null
                ? NotFound()
                : Ok(value: result.Content);
        }
        catch (ContentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest();
        }
        catch (ContentManagementSecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}