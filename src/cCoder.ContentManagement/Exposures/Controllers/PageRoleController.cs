using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PageRole = cCoder.Data.Models.Security.PageRole;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageRoleController : ODataController
{
    protected IPageRoleOrchestrationService Service { get; }

    public PageRoleController(IPageRoleOrchestrationService service, ILogger<PageRoleController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok(new MetadataContainer(typeof(PageRole), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        return Ok(Service.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PageRole entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.AddAsync(entity));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<PageRole> items)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        await Service.DeleteAllAsync(items);
        return Ok();
    }
}
