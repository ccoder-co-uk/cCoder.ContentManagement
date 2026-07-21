using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations;

internal sealed class ContentManagementMetadataTypeService : IContentManagementMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata()
    {
        return new MetadataContainerSet[2]
        {
            ContentManagementTypes(),
            SystemTypes()
        }.OrderBy((MetadataContainerSet set) => set.Name).ToArray();
    }

    private static MetadataContainerSet ContentManagementTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "ContentManagement";
        metadataContainerSet.UriBase = "ContentManagement";
        metadataContainerSet.Types = new ExtendedMetadataContainer[19]
        {
            Entity<App>(),
            Entity<Layout>(),
            Entity<Template>(),
            Entity<Page>(),
            Entity<PageInfo>(),
            Entity<Content>(),
            Entity<Component>(),
            Entity<CommonObject>(),
            Entity<Script>(),
            Entity<MetaItem>(),
            Entity<Resource>(),
            Entity<Submission>(),
            Entity<Culture>(),
            Entity<AppCulture>(),
            Entity<PageRole>(),
            Complex<RenderResult>(),
            Complex<Result<string>>(),
            Complex<Result<CommonObject>>(),
            Complex<FileContentResult>()
        }.OrderBy((ExtendedMetadataContainer type) => type.Name).ToArray();
        return metadataContainerSet;
    }

    private static MetadataContainerSet SystemTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "System";
        metadataContainerSet.Types = new ExtendedMetadataContainer[14]
        {
            new ExtendedMetadataContainer(typeof(int)),
            new ExtendedMetadataContainer(typeof(string)),
            new ExtendedMetadataContainer(typeof(decimal)),
            new ExtendedMetadataContainer(typeof(double)),
            new ExtendedMetadataContainer(typeof(float)),
            new ExtendedMetadataContainer(typeof(bool)),
            new ExtendedMetadataContainer(typeof(DateTime)),
            new ExtendedMetadataContainer(typeof(DateTimeOffset)),
            new ExtendedMetadataContainer(typeof(TimeSpan)),
            new ExtendedMetadataContainer(typeof(IEnumerable<object>)),
            new ExtendedMetadataContainer(typeof(ICollection<object>)),
            new ExtendedMetadataContainer(typeof(IDictionary<string, object>)),
            new ExtendedMetadataContainer(typeof(object)),
            new ExtendedMetadataContainer(typeof(Guid))
        }.Select(type =>
        {
            type.Category = "System";
            return type;
        }).ToArray();
        return metadataContainerSet;
    }

    private static ExtendedMetadataContainer Entity<T>()
    {
        return Create(typeof(T), "ContentManagement", isEntity: true, hasEndpoint: true);
    }

    private static ExtendedMetadataContainer Complex<T>()
    {
        return Create(typeof(T), "ContentManagement");
    }

    private static ExtendedMetadataContainer Create(Type type, string category, bool isEntity = false, bool hasEndpoint = false)
    {
        return new ExtendedMetadataContainer(type, isEntity, hasEndpoint)
        {
            Category = category
        };
    }
}
