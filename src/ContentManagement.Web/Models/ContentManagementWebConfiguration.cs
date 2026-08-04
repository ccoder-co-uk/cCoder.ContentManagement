// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;

namespace ContentManagement.Web.Models;

public sealed class ContentManagementWebConfiguration
{
    public ContentManagementConfiguration ContentManagement { get; set; }

    public DataConfiguration Data { get; set; }

    public SecurityConfiguration Security { get; set; }

    public AppSecurityConfiguration AppSecurity { get; set; }

    public EventingConfiguration Eventing { get; set; }

}