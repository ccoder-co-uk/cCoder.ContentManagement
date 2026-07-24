// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Foundations.Storages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class AppController : ODataController
{
    private readonly IAppManager manager;

    public AppController(IAppManager manager) =>
        this.manager = manager;

    [HttpGet]
    [ActionName("IsAdmin")]
    public IActionResult GetIsAdmin([FromRoute] int key, string userName) =>
        Ok(value: manager.IsAdmin(appId: key, userName: userName));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    [ActionName("Users")]
    public IActionResult GetUsers([FromRoute] int key) =>
        Ok(value: manager.GetUsers(appId: key));

    [HttpPost]
    [ActionName("UpdatePageOrder")]
    public async Task<IActionResult> PostUpdatePageOrderAsync([FromRoute] int key, ODataActionParameters p)
    {
        App app = p["app"] as App;
        await manager.UpdatePageOrderAsync(appId: key, updatedApp: app);
        return Ok();
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(App)) : new MetadataContainer(type: typeof(App), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<App> queryOptions) =>
        Ok(value: manager.GetAll());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<App> result = manager.GetAll()
                .Where(predicate: app => app.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] App newApp)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: CreateResponseApp(newApp: await manager.AddAsync(newApp: newApp)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] App updatedApp)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        updatedApp.Id = key;
        return Ok(value: CreateResponseApp(newApp: await manager.UpdateAsync(updatedApp: updatedApp)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    [ActionName("Patch")]
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<App> updatedApp)
    {
        App originalEntity = manager.Get(appManagerId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedApp.Patch(original: originalEntity);
        return Ok(value: CreateResponseApp(newApp: await manager.UpdateAsync(updatedApp: originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await manager.DeleteAsync(appId: key);
        return Ok();
    }

    private static App CreateResponseApp(App newApp)
    {
        if (newApp == null)
        {
            return null;
        }

        return new App
        {
            Id = newApp.Id,
            DefaultCultureId = newApp.DefaultCultureId,
            TenantId = newApp.TenantId,
            Name = newApp.Name,
            Domain = newApp.Domain,
            DefaultTheme = newApp.DefaultTheme,
            ConfigJson = newApp.ConfigJson
        };
    }
}