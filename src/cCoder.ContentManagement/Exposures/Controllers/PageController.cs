// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Models;
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
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageController : ODataController
{
    private readonly IPageOrchestrationService service;
    private readonly IPageRenderCoordinationService renderService;

    public PageController(
        IPageOrchestrationService service,
        IPageRenderCoordinationService renderService,
        ILogger<PageController> log)
    {
        this.service = service;
        this.renderService = renderService;
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    public IActionResult Get(ODataQueryOptions<Page> queryOptions) =>
        Ok(value: service.GetAllPage());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    [ActionName("RootFor")]
    public IActionResult GetRootFor([FromRoute] int key) =>
        Ok(value: CreateResponsePage(newPage: service.GetRootPage(pageId: key)));

    [HttpGet]
    [ActionName("Menu")]
    public IActionResult GetMenu([FromRoute] int key, string culture) =>
        Ok(value: new Result<string>
        {
            Id = key.ToString(),
            Item = service.MenuFor(pageId: key, culture: culture),
            Success = true
        });

    [HttpGet]
    [AllowAnonymous]
    [ActionName("Render")]
    public IActionResult GetRender(int appId, string path, string theme, string culture) =>
        Ok(value: renderService.RenderRenderResult(appId: appId, path: path, theme: theme, culture: culture));

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Page)) : new MetadataContainer(type: typeof(Page), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<Page> result = service.GetAllPage()
                .Where(predicate: page => page.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Page newPage)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: CreateResponsePage(newPage: await service.AddPageAsync(newPage: newPage)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Page updatedPage)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        updatedPage.Id = key;
        return Ok(value: CreateResponsePage(newPage: await service.UpdatePageAsync(updatedPage: updatedPage)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    [ActionName("Patch")]
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<Page> updatedPage)
    {
        Page originalEntity = service.GetPage(pageId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedPage.Patch(original: originalEntity);
        return Ok(value: CreateResponsePage(newPage: await service.UpdatePageAsync(updatedPage: originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await service.DeleteAsync(pageId: key);
        return Ok();
    }

    private static Page CreateResponsePage(Page newPage)
    {
        if (newPage == null)
        {
            return null;
        }

        return new Page
        {
            Id = newPage.Id,
            ParentId = newPage.ParentId,
            AppId = newPage.AppId,
            Order = newPage.Order,
            ShowOnMenus = newPage.ShowOnMenus,
            Name = newPage.Name,
            LastUpdated = newPage.LastUpdated,
            LastUpdatedBy = newPage.LastUpdatedBy,
            CreatedOn = newPage.CreatedOn,
            CreatedBy = newPage.CreatedBy,
            Path = newPage.Path,
            ResourceKey = newPage.ResourceKey,
            Layout = newPage.Layout
        };
    }
}