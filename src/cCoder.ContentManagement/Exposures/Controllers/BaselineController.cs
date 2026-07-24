// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.ContentManagement.Exposures.Controllers;

[ApiController]
[Route("Api/ContentManagement/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: UIBaseline.Packages);
}