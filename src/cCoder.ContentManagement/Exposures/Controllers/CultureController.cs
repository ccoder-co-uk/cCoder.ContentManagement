// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class CultureController : ODataController
{
    protected ICultureOrchestrationService Service { get; }

    public CultureController(ICultureOrchestrationService service, ILogger<CultureController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Culture)) : new MetadataContainer(type: typeof(Culture), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Culture> queryOptions) =>
        Ok(value: Service.GetAllCulture());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] string key)
    {
        try
        {
            IQueryable<Culture> result = Service.GetAllCulture()
                .Where(predicate: culture => culture.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Culture newCulture)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: CreateResponseCulture(newCulture: await Service.AddCultureAsync(newCulture: newCulture)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] Culture updatedCulture)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: CreateResponseCulture(newCulture: await Service.UpdateCultureAsync(updatedCulture: updatedCulture)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] string key, Delta<Culture> delta)
    {
        Culture originalEntity = Service.GetCulture(cultureId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: CreateResponseCulture(newCulture: await Service.UpdateCultureAsync(updatedCulture: originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        await Service.DeleteAsync(cultureId: key);
        return Ok();
    }

    private static Culture CreateResponseCulture(Culture newCulture)
    {
        if (newCulture == null)
        {
            return null;
        }

        return new Culture
        {
            Id = newCulture.Id,
            Name = newCulture.Name
        };
    }
}