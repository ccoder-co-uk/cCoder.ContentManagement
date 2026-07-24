// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;

namespace cCoder.ContentManagement.Api.OData;

public class ODataModel
{
    public string Context { get; set; }
    public string Description { get; set; }
    public IEdmModel EDMModel { get; set; }

    public ODataModel
()
    {
        this.Context = string.Empty;
        this.Description = string.Empty;
        this.EDMModel = null;
    }
}