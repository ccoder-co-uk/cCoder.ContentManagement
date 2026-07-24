// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.ContentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class CommonObjectController(ICommonObjectOrchestrationService service) : ODataController()
{
    protected ICommonObjectOrchestrationService Service { get; } = service;

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    public IActionResult Latest(string type) =>
        Ok(value: Service.LatestCommonObject(type: type));

    [HttpPost]
    public async Task<IActionResult> ImportAsync([FromBody] JsonElement payload)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        IEnumerable<CommonObject> items = DeserializeCommonObjects(payload: payload);

        if (items == null)
        {
            return BadRequest(message: "A common object payload is required.");
        }

        return Ok(value: await Service.ImportCommonObjectResultAsync(items: items));
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBuilder().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(CommonObject)) : new MetadataContainer(type: typeof(CommonObject), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<CommonObject> queryOptions) =>
        Ok(value: Service.GetAllCommonObject());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<CommonObject> result = Service.GetAllCommonObject()
                .Where(predicate: commonObject => commonObject.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Post([FromBody] CommonObject newCommonObject)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: await Service.AddCommonObjectAsync(newCommonObject: newCommonObject));
    }

    [HttpPut]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] CommonObject updatedCommonObject)
    {
        if (!base.ModelState.IsValid)
        {
            return new BadRequestResult(modelState: base.ModelState);
        }

        updatedCommonObject.Id = key;
        return Ok(value: await Service.UpdateCommonObjectAsync(updatedCommonObject: updatedCommonObject));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    public async Task<IActionResult> Patch([FromRoute] int key, Delta<CommonObject> delta)
    {
        CommonObject originalEntity = Service.GetCommonObject(commonObjectId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await Service.UpdateCommonObjectAsync(updatedCommonObject: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await Service.DeleteAsync(commonObjectId: key);
        return Ok();
    }

    private static IEnumerable<CommonObject> DeserializeCommonObjects(JsonElement payload)
    {
        JsonElement itemsPayload = payload.ValueKind == JsonValueKind.Object &&
                                   payload.TryGetProperty(propertyName: "value", value: out JsonElement valueElement)
            ? valueElement
            : payload;

        return itemsPayload.ValueKind switch
        {
            JsonValueKind.Array => JsonSerializer.Deserialize<CommonObject[]>(
json: itemsPayload.GetRawText(),
options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
            JsonValueKind.Null => null,
            var ignoredRequest => null
        };
    }
}