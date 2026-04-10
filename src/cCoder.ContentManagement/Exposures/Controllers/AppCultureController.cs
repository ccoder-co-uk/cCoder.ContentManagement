using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using AppCulture = cCoder.Data.Models.CMS.AppCulture;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class AppCultureController : ODataController
{
    protected IAppCultureOrchestrationService Service { get; }

    public AppCultureController(IAppCultureOrchestrationService service, ILogger<AppCultureController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok(new MetadataContainer(typeof(AppCulture), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        return Ok(Service.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AppCulture entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.AddAsync(entity));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<AppCulture> items)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        await Service.DeleteAllAsync(items);
        return Ok();
    }
}
