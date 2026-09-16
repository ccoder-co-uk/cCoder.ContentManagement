// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.ContentManagement.Exposures.Controllers;

public class PageRenderCacheController(
    IPageRenderCacheAggregationService manager,
    ILoggingBroker loggingBroker) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IActionResult Get()
    {
        try
        {
            return Ok(value: manager.GetAllPageRenderCaches());
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
            PageRenderCache result = manager.GetPageRenderCache(pageRenderCacheId: key);
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
                value: await manager.AddPageRenderCacheAsync(newPageRenderCache: newPageRenderCache));
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
            return Ok(value: await manager.UpdatePageRenderCacheAsync(updatedPageRenderCache: updatedPageRenderCache));
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
            await manager.DeletePageRenderCacheAsync(pageRenderCacheId: key);
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

}