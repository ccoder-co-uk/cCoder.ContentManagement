// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;

namespace cCoder.ContentManagement.Services.Foundations;

internal interface IContentManagementMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}