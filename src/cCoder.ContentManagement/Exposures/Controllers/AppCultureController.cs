// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Extensions.OData;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class AppCultureController : ODataController
{
    private readonly IAppCultureOrchestrationService service;

    public AppCultureController(IAppCultureOrchestrationService service, ILogger<AppCultureController> log)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: new MetadataContainer(type: typeof(AppCulture), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll() =>
        Ok(value: service.GetAllAppCulture());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AppCulture newAppCulture)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: await service.AddAppCultureAsync(newAppCulture: newAppCulture));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<AppCulture> deletedAppCulture)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        await service.DeleteAllAppCultureAsync(deletedAppCulture: deletedAppCulture);
        return Ok();
    }
}