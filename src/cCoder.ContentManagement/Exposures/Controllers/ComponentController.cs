using System.Security;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Component = cCoder.Data.Models.CMS.Component;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class ComponentController : ODataController
{
    protected IComponentOrchestrationService Service { get; }
    protected IComponentRenderer Renderer { get; }

    public ComponentController(
        IComponentOrchestrationService service,
        IComponentRenderer renderer,
        ILogger<ComponentController> log)
    {
        Service = service;
        Renderer = renderer;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Render(int appId, string name, string culture, string theme)
    {
        return Ok(Renderer.Render(appId, name, culture, theme));
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(Component)) : new MetadataContainer(typeof(Component), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Component> queryOptions)
    {
        return Ok(Service.GetAll());
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Component> result = Service.GetAll().Where(component => component.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Component entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.AddAsync(entity));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Component entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.UpdateAsync(entity));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<Component> delta)
    {
        Component originalEntity = Service.Get(key);
        if (originalEntity == null)
        {
            return NotFound();
        }
        delta.Patch(originalEntity);
        return Ok(await Service.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }
}

