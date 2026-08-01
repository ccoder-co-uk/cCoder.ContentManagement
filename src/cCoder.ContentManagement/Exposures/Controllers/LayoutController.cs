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

public class LayoutController : ODataController
{
    private readonly ILayoutManager service;

    public LayoutController(ILayoutManager service, ILogger<LayoutController> log)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
            .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Layout)) : new MetadataContainer(type: typeof(Layout), isEntity: true, hasEndpoint: true));
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
            return Ok(value: service.GetAllLayout());
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
            Layout result = service.GetAllLayout()
                .FirstOrDefault(predicate: layout => layout.Id == key);

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
    public async Task<IActionResult> Post([FromBody] Layout newLayout)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            return StatusCode(statusCode: StatusCodes.Status201Created, value: await service.AddLayoutAsync(newLayout: newLayout));
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
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Layout updatedLayout)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            return Ok(value: await service.UpdateLayoutAsync(updatedLayout: updatedLayout));
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
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<Layout> updatedLayout)
    {
        try
        {
            Layout originalEntity = service.GetLayout(layoutId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedLayout.Patch(original: originalEntity);
            return Ok(value: await service.UpdateLayoutAsync(updatedLayout: originalEntity));
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
            await service.DeleteAsync(layoutId: key);
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