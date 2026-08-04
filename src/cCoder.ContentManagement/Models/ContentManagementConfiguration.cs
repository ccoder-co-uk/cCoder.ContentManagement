// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Models;

public class ContentManagementConfiguration
{
    public string ConnectionString { get; set; }
    public int? SslPort { get; set; }
    public string WorkflowServiceUrl { get; set; }
    public string CacheSource { get; set; }
    public int? CacheSourceAppId { get; set; }
    public int CacheExpiry { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public bool IncludeLegacyCoreContext { get; set; }
    public EventProvider[] EventProviders { get; set; }
}