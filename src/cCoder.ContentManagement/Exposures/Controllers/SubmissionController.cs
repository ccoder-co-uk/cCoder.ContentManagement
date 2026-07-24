// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
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
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class SubmissionController : ODataController
{
    protected ISubmissionOrchestrationService Service { get; }

    public SubmissionController(ISubmissionOrchestrationService service, ILogger<SubmissionController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(Submission)) : new MetadataContainer(type: typeof(Submission), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<Submission> queryOptions) =>
        Ok(value: Service.GetAllSubmission());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<Submission> result = Service.GetAllSubmission()
                .Where(predicate: submission => submission.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] Submission newSubmission)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return new JsonResult(value: CreateResponseSubmission(newSubmission: await Service.AddSubmissionAsync(newSubmission: newSubmission)));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Submission updatedSubmission)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return new JsonResult(value: CreateResponseSubmission(newSubmission: await Service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission)));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<Submission> delta)
    {
        Submission originalEntity = Service.GetSubmission(submissionId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return new JsonResult(value: CreateResponseSubmission(newSubmission: await Service.UpdateSubmissionAsync(updatedSubmission: originalEntity)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await Service.DeleteAsync(submissionId: key);
        return Ok();
    }

    private static Submission CreateResponseSubmission(Submission newSubmission)
    {
        if (newSubmission == null)
        {
            return null;
        }

        return new Submission
        {
            Id = newSubmission.Id,
            AppId = newSubmission.AppId,
            CreatedBy = newSubmission.CreatedBy,
            LastUpdatedBy = newSubmission.LastUpdatedBy,
            CreatedOn = newSubmission.CreatedOn,
            LastUpdatedOn = newSubmission.LastUpdatedOn,
            SourceComponent = newSubmission.SourceComponent,
            State = newSubmission.State,
            DataJson = newSubmission.DataJson
        };
    }
}