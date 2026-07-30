// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;

namespace cCoder.ContentManagement.Models.OData;

public class MetadataContainerSet
{
    public string Name { get; set; }

    public string UriBase { get; set; }

    public ExtendedMetadataContainer[] Types { get; set; }

}