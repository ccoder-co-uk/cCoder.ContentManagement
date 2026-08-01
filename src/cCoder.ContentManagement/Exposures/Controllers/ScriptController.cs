// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Extensions.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class ScriptController : ODataController
{
    private readonly IScriptManager service;

    public ScriptController(IScriptManager service, ILogger<ScriptController> log)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
            .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Script)) : new MetadataContainer(type: typeof(Script), isEntity: true, hasEndpoint: true));
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(value: service.GetAllScript());
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
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
            Script result = service.GetAllScript()
                .FirstOrDefault(predicate: script => script.Id == key);

            return result is null
                ? NotFound()
                : Ok(value: result);
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Script newScript)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            return StatusCode(statusCode: StatusCodes.Status201Created, value: await service.AddScriptAsync(newScript: newScript));
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Script updatedScript)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            return Ok(value: await service.UpdateScriptAsync(updatedScript: updatedScript));
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    [ActionName("Patch")]
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<Script> updatedScript)
    {
        try
        {
            Script originalEntity = service.GetScript(scriptId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedScript.Patch(original: originalEntity);
            return Ok(value: await service.UpdateScriptAsync(updatedScript: originalEntity));
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        try
        {
            await service.DeleteAsync(scriptId: key);
            return NoContent();
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}