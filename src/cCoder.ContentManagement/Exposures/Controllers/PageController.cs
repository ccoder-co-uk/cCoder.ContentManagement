using System.Security;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Page = cCoder.Data.Models.CMS.Page;
using StringResult = cCoder.ContentManagement.Models.Result<string>;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageController : ODataController
{
    protected IPageOrchestrationService Service { get; }
    private IPageRenderCoordinationService RenderService { get; }

    public PageController(
        IPageOrchestrationService service,
        IPageRenderCoordinationService renderService,
        ILogger<PageController> log)
    {
        Service = service;
        RenderService = renderService;
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    public IActionResult Get(ODataQueryOptions<Page> queryOptions)
    {
        return Ok(Service.GetAll());
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    public IActionResult RootFor([FromRoute] int key)
    {
        return Ok(CreateResponsePage(Service.GetRoot(key)));
    }

    [HttpGet]
    public IActionResult Menu([FromRoute] int key, string culture)
    {
        return Ok(new StringResult
        {
            Id = key.ToString(),
            Item = Service.MenuFor(key, culture),
            Success = true
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Render(int appId, string path, string theme, string culture)
    {
        return Ok(RenderService.Render(appId, path, theme, culture));
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(Page)) : new MetadataContainer(typeof(Page), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Page> result = Service.GetAll().Where(page => page.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Page entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(CreateResponsePage(await Service.AddAsync(entity)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Page entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        entity.Id = key;
        return Ok(CreateResponsePage(await Service.UpdateAsync(entity)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<Page> delta)
    {
        Page originalEntity = Service.Get(key);
        if (originalEntity == null)
        {
            return NotFound();
        }
        delta.Patch(originalEntity);
        return Ok(CreateResponsePage(await Service.UpdateAsync(originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }

    private static Page CreateResponsePage(Page page)
    {
        if (page == null)
        {
            return null;
        }

        return new Page
        {
            Id = page.Id,
            ParentId = page.ParentId,
            AppId = page.AppId,
            Order = page.Order,
            ShowOnMenus = page.ShowOnMenus,
            Name = page.Name,
            LastUpdated = page.LastUpdated,
            LastUpdatedBy = page.LastUpdatedBy,
            CreatedOn = page.CreatedOn,
            CreatedBy = page.CreatedBy,
            Path = page.Path,
            ResourceKey = page.ResourceKey,
            Layout = page.Layout
        };
    }
}

