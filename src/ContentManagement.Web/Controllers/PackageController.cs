// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models.Exceptions;
using cCoder.Data.Models.Packaging;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Web.Controllers;

[ApiController]
[Route("Api/ContentManagement/Package")]
public sealed class PackageController(
    IContentManagementPackageManager contentManagementPackageManager) : ControllerBase
{
    [HttpPost("Import")]
    public async Task<IActionResult> PostImportAsync([FromQuery] int appId, [FromBody] Package newPackage)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await contentManagementPackageManager.ImportPackageAsync(
                appId: appId,
                package: newPackage);

            return Ok();
        }
        catch (ContentManagementValidationException)
        {
            return BadRequest();
        }
        catch (ContentManagementSecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}