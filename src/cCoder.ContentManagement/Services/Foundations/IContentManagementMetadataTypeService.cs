using cCoder.ContentManagement.Api.OData;

namespace cCoder.ContentManagement.Services.Foundations;

internal interface IContentManagementMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}
