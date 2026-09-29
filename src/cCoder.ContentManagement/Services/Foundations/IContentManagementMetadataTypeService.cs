// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.OData;
using System.Collections.Generic;

namespace cCoder.ContentManagement.Services.Foundations;

internal interface IContentManagementMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();

    IEnumerable<string> GetKnownMetadataPayloads();
}