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

public class ContentController(IContentOrchestrationService contentOrchestrationService, ILogger<ContentController> log) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(Content)) : new MetadataContainer(typeof(Content), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Content> queryOptions) =>
        Ok(contentOrchestrationService.GetAll());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Content> result = contentOrchestrationService.GetAll()
                .Where(content => content.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Content entity)
    {
        if (!ModelState.IsValid)
            return new BadRequestResult(ModelState);

        return Ok(await contentOrchestrationService.AddAsync(entity));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Content entity)
    {
        if (!ModelState.IsValid)
            return new BadRequestResult(ModelState);

        entity.Id = key;
        return Ok(await contentOrchestrationService.UpdateAsync(entity));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<Content> delta)
    {
        Content originalEntity = contentOrchestrationService.Get(key);
        if (originalEntity == null)
            return NotFound();

        delta.Patch(originalEntity);
        return Ok(await contentOrchestrationService.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await contentOrchestrationService.DeleteAsync(key);
        return Ok();
    }
}
