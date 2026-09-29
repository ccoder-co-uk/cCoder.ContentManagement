// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations;
using cCoder.ContentManagement.Brokers.OData;


namespace cCoder.Core.Services.Tests.CMS.Foundations;

public partial class ContentManagementMetadataTypeServiceTests
{
    private readonly IContentManagementMetadataTypeService service;

    public ContentManagementMetadataTypeServiceTests() =>
        service = new ContentManagementMetadataTypeService(
            metadataTypeBroker: new MetadataTypeBroker());
}