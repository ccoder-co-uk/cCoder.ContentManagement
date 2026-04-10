using System.Security;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Brokers;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class AppController : ODataController
{
    protected IAppOrchestrationService Service { get; }

    protected IAuthorizationBroker AuthorizationBroker { get; }

    public AppController(IAppOrchestrationService service, IAuthorizationBroker authorizationBroker, ILogger<AppController> log)
    {
        Service = service;
        AuthorizationBroker = authorizationBroker;
    }

    [HttpGet]
    public IActionResult IsAdmin([FromRoute] int key, string userName)
    {
        return Ok(AuthorizationBroker.IsAdmin(key, userName));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    public IActionResult Users([FromRoute] int key)
    {
        return Ok(Service.GetAppUsers(key));
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePageOrderAsync([FromRoute] int key, ODataActionParameters p)
    {
        App app = p["app"] as App;
        await Service.UpdatePageOrderAsync(key, app);
        return Ok();
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(App)) : new MetadataContainer(typeof(App), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<App> queryOptions)
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
            IQueryable<App> result = Service.GetAll().Where(app => app.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] App entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(CreateResponseApp(await Service.AddAsync(entity)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] App entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        entity.Id = key;
        return Ok(CreateResponseApp(await Service.UpdateAsync(entity)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<App> delta)
    {
        App originalEntity = Service.Get(key);
        if (originalEntity == null)
        {
            return NotFound();
        }
        delta.Patch(originalEntity);
        return Ok(CreateResponseApp(await Service.UpdateAsync(originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }

    private static App CreateResponseApp(App app)
    {
        if (app == null)
        {
            return null;
        }

        return new App
        {
            Id = app.Id,
            DefaultCultureId = app.DefaultCultureId,
            TenantId = app.TenantId,
            Name = app.Name,
            Domain = app.Domain,
            DefaultTheme = app.DefaultTheme,
            ConfigJson = app.ConfigJson
        };
    }
}

