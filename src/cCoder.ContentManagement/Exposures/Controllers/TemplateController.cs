using System.Security;
using System.Text;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Orchestrations;
using iText.Html2pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Newtonsoft.Json;
using Template = cCoder.Data.Models.CMS.Template;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class TemplateController : ODataController
{
    protected ITemplateOrchestrationService Service { get; }
    protected ITemplateRenderer Renderer { get; }

    public TemplateController(
        ITemplateOrchestrationService service,
        ITemplateRenderer renderer,
        ILogger<TemplateController> log)
    {
        Service = service;
        Renderer = renderer;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Render(int appId, string name, string culture)
    {
        using StreamReader reader = new StreamReader(base.Request.Body);
        dynamic m = JsonConvert.DeserializeObject(await reader.ReadToEndAsync());
        return Content(Renderer.Render(appId, name, culture, m), "text/plain", Encoding.UTF8);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> HtmlToPdf(string name)
    {
        using StreamReader reader = new StreamReader(base.Request.Body);
        string htmlContent = await reader.ReadToEndAsync();
        using MemoryStream pdfStream = new MemoryStream();
        HtmlConverter.ConvertToPdf(htmlContent, (Stream)pdfStream);
        return File(pdfStream.ToArray(), "application/pdf", name + ".pdf");
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(Template)) : new MetadataContainer(typeof(Template), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Template> queryOptions)
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
            IQueryable<Template> result = Service.GetAll().Where(template => template.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Template entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.AddAsync(entity));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Template entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return Ok(await Service.UpdateAsync(entity));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<Template> delta)
    {
        Template originalEntity = Service.Get(key);
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

