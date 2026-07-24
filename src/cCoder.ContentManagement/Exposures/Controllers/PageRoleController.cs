// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageRoleController : ODataController
{
    private readonly IPageRoleOrchestrationService service;

    public PageRoleController(IPageRoleOrchestrationService service, ILogger<PageRoleController> log)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: new MetadataContainer(type: typeof(PageRole), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll() =>
        Ok(value: service.GetAllPageRole());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PageRole newPageRole)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: await service.AddPageRoleAsync(newPageRole: newPageRole));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<PageRole> deletedPageRole)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        await service.DeleteAllPageRoleAsync(deletedPageRole: deletedPageRole);
        return Ok();
    }
}