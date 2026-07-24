// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models;

public class Config
{
    public IDictionary<string, string> ConnectionStrings { get; set; }
    public IDictionary<string, string> Settings { get; set; }
    public IDictionary<string, string> Services { get; set; }
    public bool DebugInfo { get; set; }

    public bool LogSQL { get; set; }

    public Config
    ()
    {
        this.ConnectionStrings = new Dictionary<string, string>();
        this.Settings = new Dictionary<string, string>();
        this.Services = new Dictionary<string, string>();
    }
}