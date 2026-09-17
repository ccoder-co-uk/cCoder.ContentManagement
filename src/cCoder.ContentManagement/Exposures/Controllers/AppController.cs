// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Models.Exceptions;
using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Aggregations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class AppController : ODataController
{
    private readonly ILoggingBroker loggingBroker;
    private readonly IAppManagerAggregationService manager;

    public AppController(IAppManagerAggregationService manager, ILoggingBroker loggingBroker)
    {
        this.manager = manager;
        this.loggingBroker = loggingBroker;
    }

    [HttpGet]
    [ActionName("IsAdmin")]
    public IActionResult GetIsAdmin([FromRoute] int key, string userName)
    {
        try
        {
            if (key <= 0 || string.IsNullOrWhiteSpace(value: userName))
            {
                return NotFound();
            }

            return Ok(value: manager.GetAdminAppManagerContext(
                appManagerContext: new AppManagerContext
                {
                    AppId = key,
                    UserName = userName
                })
            .IsAdmin);
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    [ActionName("Users")]
    public IActionResult GetUsers([FromRoute] int key)
    {
        try
        {
            if (key <= 0)
            {
                return NotFound();
            }

            return Ok(value: manager.GetUsersAppManagerContext(
                appManagerContext: new AppManagerContext { AppId = key })
            .Users);
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [ActionName("UpdatePageOrder")]
    public async Task<IActionResult> PostUpdatePageOrderAsync([FromRoute] int key, ODataActionParameters p)
    {
        try
        {
            App app = p["app"] as App;

            await manager.UpdatePageOrderAppManagerContextAsync(
                updatedAppManagerContext: new AppManagerContext
                {
                    AppId = key,
                    App = app
                });

            return Ok();
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<App> queryOptions)
    {
        try
        {
            return Ok(value: manager.GetAllAppManagerContext(
                appManagerContext: new AppManagerContext())
            .Apps);
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            App result = manager.GetAllAppManagerContext(
                appManagerContext: new AppManagerContext())
            .Apps
                .FirstOrDefault(predicate: app => app.Id == key);

            return result is null
                ? NotFound()
                : Ok(value: result);
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] App newApp)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            AppManagerContext result = await manager.AddAppManagerContextAsync(
                newAppManagerContext: new AppManagerContext { App = newApp });

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: CreateResponseApp(newApp: result.App));
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] App updatedApp)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            updatedApp.Id = key;

            AppManagerContext result = await manager.UpdateAppManagerContextAsync(
                updatedAppManagerContext: new AppManagerContext { App = updatedApp });

            return Ok(value: CreateResponseApp(newApp: result.App));
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await manager.DeleteAppManagerContextAsync(
                deletedAppManagerContext: new AppManagerContext { AppId = key });

            return Accepted();
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

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static App CreateResponseApp(App newApp)
    {
        if (newApp == null)
        {
            return null;
        }

        return new App
        {
            Id = newApp.Id,
            DefaultCultureId = newApp.DefaultCultureId,
            TenantId = newApp.TenantId,
            Name = newApp.Name,
            Domain = newApp.Domain,
            DefaultTheme = newApp.DefaultTheme,
            ConfigJson = newApp.ConfigJson
        };
    }
}