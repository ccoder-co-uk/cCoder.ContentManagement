// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
[Route(template: "Api/ContentManagement/App({key})/Users()")]
public sealed class AppUserController(
    IAppUserOrchestrationService service,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6)]
    public IActionResult GetUsers([FromRoute] int key)
    {
        try
        {
            if (key <= 0)
            {
                return NotFound();
            }

            return Ok(value: new
            {
                value = service.GetAllUsers(appId: key)
            });
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