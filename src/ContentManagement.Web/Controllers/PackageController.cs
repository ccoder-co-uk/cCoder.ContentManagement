// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.Data.Models.Packaging;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Web.Controllers;

[ApiController]
[Route("Api/ContentManagement/Package")]
[Route("Api/Core/Package")]
public sealed class PackageController(
    IContentManagementMigrationAggregationService contentManagementMigrationAggregationService) : ControllerBase
{
    [HttpPost("Import")]
    public async Task<IActionResult> PostImportAsync([FromQuery] int appId, [FromBody] Package newPackage)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(modelState: ModelState);
        }

        await contentManagementMigrationAggregationService.ImportPackageAsync(
            appId: appId,
            package: newPackage);

        return Ok();
    }
}