// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Brokers.OData;
using cCoder.ContentManagement.Models.Exceptions;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;
using System.Security;
using System.Security.Authentication;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class TemplateController : ODataController
{
    private readonly ILoggingBroker loggingBroker;
    private readonly ITemplateOrchestrationService service;
    private readonly IODataResultBroker oDataResultBroker;

    public TemplateController(
        ITemplateOrchestrationService service,
        ILoggingBroker loggingBroker,
        IODataResultBroker oDataResultBroker)
    {
        this.service = service;
        this.loggingBroker = loggingBroker;
        this.oDataResultBroker = oDataResultBroker;
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {

            {
                var actionValue = service.GetAllTemplates();
                loggingBroker.LogInformation(message: "Controller action completed successfully.");
                return Ok(value: actionValue);
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

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Template> query = service.GetAllTemplates()
                .Where(predicate: template => template.Id == key);

            int? result = query
                .Select(selector: template => (int?)template.Id)
                .FirstOrDefault();


            if (result is null)

            {

                loggingBroker.LogWarning(message: "Controller action returned a client error.");

                return NotFound();

            }

            else

            {

                var actionValue = oDataResultBroker.CreateSingleResult(queryable: query);

                loggingBroker.LogInformation(message: "Controller action completed successfully.");

                return Ok(value: actionValue);

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

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Template newTemplate)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {

                {
                    var actionValue = new BadRequestResult(modelState: base.ModelState);
                    loggingBroker.LogWarning(message: "Controller action returned a client error.");
                    return actionValue;
                }
            }


            {

                var actionValue = await service.AddTemplateAsync(newTemplate: newTemplate);

                loggingBroker.LogInformation(message: "Controller action completed successfully.");

                return StatusCode(statusCode: StatusCodes.Status201Created, value: actionValue);

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

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Template updatedTemplate)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {

                {
                    var actionValue = new BadRequestResult(modelState: base.ModelState);
                    loggingBroker.LogWarning(message: "Controller action returned a client error.");
                    return actionValue;
                }
            }


            {

                var actionValue = await service.UpdateTemplateAsync(updatedTemplate: updatedTemplate);

                loggingBroker.LogInformation(message: "Controller action completed successfully.");

                return Ok(value: actionValue);

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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(templateId: key);

            {
                loggingBroker.LogInformation(message: "Controller action completed successfully.");
                return NoContent();
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