using System.Security;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class SubmissionController : ODataController
{
    protected ISubmissionOrchestrationService Service { get; }

    public SubmissionController(ISubmissionOrchestrationService service, ILogger<SubmissionController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok((base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build().EDMModel.GetExtendedMetadataForType("Core", typeof(Submission)) : new MetadataContainer(typeof(Submission), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Submission> queryOptions)
    {
        return Ok(Service.GetAll());
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<Submission> result = Service.GetAll().Where(submission => submission.Id == key);
            return Ok(SingleResult.Create(result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Submission entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return new JsonResult(CreateResponseSubmission(await Service.AddAsync(entity)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Submission entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.ContentManagement.Api.OData.BadRequestResult(base.ModelState);
        }
        return new JsonResult(CreateResponseSubmission(await Service.UpdateAsync(entity)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<Submission> delta)
    {
        Submission originalEntity = Service.Get(key);
        if (originalEntity == null)
        {
            return NotFound();
        }
        delta.Patch(originalEntity);
        return new JsonResult(CreateResponseSubmission(await Service.UpdateAsync(originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }

    private static Submission CreateResponseSubmission(Submission submission)
    {
        if (submission == null)
        {
            return null;
        }

        return new Submission
        {
            Id = submission.Id,
            AppId = submission.AppId,
            CreatedBy = submission.CreatedBy,
            LastUpdatedBy = submission.LastUpdatedBy,
            CreatedOn = submission.CreatedOn,
            LastUpdatedOn = submission.LastUpdatedOn,
            SourceComponent = submission.SourceComponent,
            State = submission.State,
            DataJson = submission.DataJson
        };
    }
}

