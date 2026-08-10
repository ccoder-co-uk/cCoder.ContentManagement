// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageRenderCacheController(
    IPageRenderCacheManager manager,
    ILoggingBroker loggingBroker) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IActionResult Get()
    {
        try
        {
            return Ok(value: manager.GetAll());
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [HttpGet]
    [EnableQuery]
    public IActionResult Get([FromRoute] string key)
    {
        try
        {
            PageRenderCache result = manager.Get(pageRenderCacheId: key);
            return result == null ? NotFound() : Ok(value: result);
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PageRenderCache newPageRenderCache)
    {
        try
        {
            if (!ModelState.IsValid) { return BadRequest(modelState: ModelState); }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await manager.AddAsync(newPageRenderCache: newPageRenderCache));
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] PageRenderCache updatedPageRenderCache)
    {
        try
        {
            if (!ModelState.IsValid) { return BadRequest(modelState: ModelState); }

            updatedPageRenderCache.Id = key;
            return Ok(value: await manager.UpdateAsync(updatedPageRenderCache: updatedPageRenderCache));
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> PutPatch([FromRoute] string key, Delta<PageRenderCache> updatedPageRenderCache)
    {
        try
        {
            PageRenderCache entity = manager.Get(pageRenderCacheId: key);

            if (entity == null)
            {
                return NotFound();
            }

            updatedPageRenderCache.Patch(original: entity);
            return Ok(value: await manager.UpdateAsync(updatedPageRenderCache: entity));
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        try
        {
            await manager.DeleteAsync(pageRenderCacheId: key);
            return NoContent();
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }

    [HttpPost]
    [ActionName("RebuildApp")]
    public async Task<IActionResult> GetRebuildApp([FromBody] Microsoft.AspNetCore.OData.Formatter.ODataActionParameters parameters)
    {
        try
        {
            return Ok(value: await manager.RebuildAppAsync(appId: (int)parameters["appId"]));
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Unable to rebuild PageRenderCache for app {AppId}.",
                args: parameters["appId"]);

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [ActionName("RebuildPage")]
    public async Task<IActionResult> GetRebuildPage([FromBody] Microsoft.AspNetCore.OData.Formatter.ODataActionParameters parameters)
    {
        try
        {
            return Ok(value: await manager.RebuildPageAsync(pageId: (int)parameters["pageId"]));
        }
        catch (ContentManagementValidationException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest(); }
        catch (ContentManagementSecurityException exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden); }
        catch (Exception exception) {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError); }
    }
}