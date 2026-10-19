// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ContentManagement.Web.Hosting;

internal sealed class AppSecurityControllerFeatureProvider
    : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(
        IEnumerable<ApplicationPart> parts,
        ControllerFeature feature) =>
        feature.Controllers.Remove(
            item: typeof(cCoder.AppSecurity.Exposures.Controllers.AppController)
                .GetTypeInfo());
}