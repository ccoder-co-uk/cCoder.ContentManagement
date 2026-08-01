// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Extensions.OData;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageRoleController : ODataController
{
    private readonly IPageRoleManager service;

    public PageRoleController(IPageRoleManager service, ILogger<PageRoleController> log)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: new MetadataContainer(type: typeof(PageRole), isEntity: true, hasEndpoint: true));
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
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(value: service.GetAllPageRole());
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
    public async Task<IActionResult> Post([FromBody] PageRole newPageRole)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            return StatusCode(statusCode: StatusCodes.Status201Created, value: await service.AddPageRoleAsync(newPageRole: newPageRole));
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
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<PageRole> deletedPageRole)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new BadRequestResult(modelState: base.ModelState);
            }

            await service.DeleteAllPageRoleAsync(deletedPageRole: deletedPageRole);
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