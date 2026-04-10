using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;

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
        metadataContainerSet.Name = "Core";
        metadataContainerSet.UriBase = "Core";
        metadataContainerSet.Types = new ExtendedMetadataContainer[19]
        {
            Entity<cCoder.Data.Models.CMS.App>(),
            Entity<cCoder.Data.Models.CMS.Layout>(),
            Entity<cCoder.Data.Models.CMS.Template>(),
            Entity<cCoder.Data.Models.CMS.Page>(),
            Entity<cCoder.Data.Models.CMS.PageInfo>(),
            Entity<cCoder.Data.Models.CMS.Content>(),
            Entity<cCoder.Data.Models.CMS.Component>(),
            Entity<cCoder.Data.Models.CommonObject>(),
            Entity<cCoder.Data.Models.CMS.Script>(),
            Entity<cCoder.Data.Models.CMS.MetaItem>(),
            Entity<cCoder.Data.Models.CMS.Resource>(),
            Entity<cCoder.Data.Models.CMS.Submission>(),
            Entity<cCoder.Data.Models.CMS.Culture>(),
            Entity<cCoder.Data.Models.CMS.AppCulture>(),
            Entity<cCoder.Data.Models.Security.PageRole>(),
            Complex<cCoder.ContentManagement.Models.RenderResult>(),
            Complex<Result<string>>(),
            Complex<Result<cCoder.Data.Models.CommonObject>>(),
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
        }.Select(delegate (ExtendedMetadataContainer type)
        {
            type.Category = "System";
            return type;
        }).ToArray();
        return metadataContainerSet;
    }

    private static ExtendedMetadataContainer Entity<T>()
    {
        return Create(typeof(T), "Core", isEntity: true, hasEndpoint: true);
    }

    private static ExtendedMetadataContainer Complex<T>()
    {
        return Create(typeof(T), "Core");
    }

    private static ExtendedMetadataContainer Create(Type type, string category, bool isEntity = false, bool hasEndpoint = false)
    {
        return new ExtendedMetadataContainer(type, isEntity, hasEndpoint)
        {
            Category = category
        };
    }
}
