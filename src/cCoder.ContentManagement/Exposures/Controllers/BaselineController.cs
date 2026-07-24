// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
[Route("Api/ContentManagement/Baseline")]
public sealed class BaselineController(
    IBaselineManager baselineManager) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: baselineManager.GetPackages());
}