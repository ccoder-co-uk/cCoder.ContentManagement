// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using BadRequestResult = cCoder.ContentManagement.Api.OData.BadRequestResult;
using System.Text.Json;
using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Extensions.OData;
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

public class CommonObjectController(ICommonObjectManager service) : ODataController()
{
    private readonly ICommonObjectManager service = service;

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.All, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 6, MaxExpansionDepth = 6)]
    [ActionName("Latest")]
    public IActionResult GetLatest(string type) =>
        Ok(value: service.LatestCommonObject(type: type));

    [HttpPost]
    [ActionName("Import")]
    public async Task<IActionResult> PostImportAsync([FromBody] JsonElement payload)
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

        return Ok(value: await service.ImportCommonObjectResultAsync(items: items));
    }

    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: (base.Request.Query["extend"] == "true") ? new ContentManagementModelBroker().Build()
        .EDMModel.GetExtendedMetadataForType(context: "ContentManagement", type: typeof(CommonObject)) : new MetadataContainer(type: typeof(CommonObject), isEntity: true, hasEndpoint: true));

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 5, MaxExpansionDepth = 5)]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<CommonObject> queryOptions) =>
        Ok(value: service.GetAllCommonObject());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            IQueryable<CommonObject> result = service.GetAllCommonObject()
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

        return Ok(value: await service.AddCommonObjectAsync(newCommonObject: newCommonObject));
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
        return Ok(value: await service.UpdateCommonObjectAsync(updatedCommonObject: updatedCommonObject));
    }

    [AcceptVerbs(new string[] { "PATCH", "MERGE" })]
    [ActionName("Patch")]
    public async Task<IActionResult> PutPatch([FromRoute] int key, Delta<CommonObject> updatedCommonObject)
    {
        CommonObject originalEntity = service.GetCommonObject(commonObjectId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedCommonObject.Patch(original: originalEntity);
        return Ok(value: await service.UpdateCommonObjectAsync(updatedCommonObject: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        await service.DeleteAsync(commonObjectId: key);
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