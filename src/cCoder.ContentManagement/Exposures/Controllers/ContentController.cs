// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Api.OData;
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

public class ContentController(IContentOrchestrationService contentOrchestrationService)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Content)) : new MetadataContainer(type: typeof(Content), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Content> queryOptions) =>
        Ok(value: contentOrchestrationService.GetAllContent());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Content> result = contentOrchestrationService.GetAllContent()
                .Where(predicate: content => content.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Content newContent)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await contentOrchestrationService.AddContentAsync(newContent: newContent));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Content updatedContent)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestResult(modelState: ModelState);
        }

        updatedContent.Id = key;
        return Ok(value: await contentOrchestrationService.UpdateContentAsync(updatedContent: updatedContent));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<Content> delta)
    {
        Content originalEntity = contentOrchestrationService.GetContent(contentId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await contentOrchestrationService.UpdateContentAsync(updatedContent: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await contentOrchestrationService.DeleteAsync(contentId: key);
        return Ok();
    }
}