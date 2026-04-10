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
    public async Task<IActionResult> ImportAsync([FromQuery] int appId, [FromBody] Package package)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await contentManagementMigrationAggregationService.ImportPackageAsync(appId, package);
        return Ok();
    }
}
