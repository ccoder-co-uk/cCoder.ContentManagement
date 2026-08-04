// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Exceptions;
using System.Security;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Extensions.OData;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class ContentController(IContentManager contentOrchestrationService)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
            .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Content)) : typeof(Content).CreateMetadataContainer(isEntity: true, hasEndpoint: true));
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
            return Ok(value: contentOrchestrationService.GetAllContent());
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
            Content result = contentOrchestrationService.GetAllContent()
                .FirstOrDefault(predicate: content => content.Id == key);

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
    public async Task<IActionResult> Post([FromBody] Content newContent)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult(modelState: ModelState);
            }

            return StatusCode(statusCode: StatusCodes.Status201Created, value: await contentOrchestrationService.AddContentAsync(newContent: newContent));
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
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Content updatedContent)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult(modelState: ModelState);
            }

            updatedContent.Id = key;
            return Ok(value: await contentOrchestrationService.UpdateContentAsync(updatedContent: updatedContent));
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
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<Content> updatedContent)
    {
        try
        {
            Content originalEntity = contentOrchestrationService.GetContent(contentId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedContent.Patch(original: originalEntity);
            return Ok(value: await contentOrchestrationService.UpdateContentAsync(updatedContent: originalEntity));
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
            await contentOrchestrationService.DeleteAsync(contentId: key);
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