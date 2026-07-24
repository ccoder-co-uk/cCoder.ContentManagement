// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Api.OData;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Foundations;

internal sealed partial class ContentManagementMetadataTypeService : IContentManagementMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch<IEnumerable<MetadataContainerSet>>(operation: () =>
    {
        ValidateKnownMetadataOnGet(inputs: []);

        return new MetadataContainerSet[2]
        {
            ContentManagementTypes(),
            SystemTypes()
        }.OrderBy(keySelector: (MetadataContainerSet set) => set.Name)
            .ToArray();

    });

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
        }.OrderBy(keySelector: (ExtendedMetadataContainer type) => type.Name)
            .ToArray();

        return metadataContainerSet;
    }

    private static MetadataContainerSet SystemTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "System";

        metadataContainerSet.Types = new ExtendedMetadataContainer[14]
        {
            new ExtendedMetadataContainer(type: typeof(int)),
            new ExtendedMetadataContainer(type: typeof(string)),
            new ExtendedMetadataContainer(type: typeof(decimal)),
            new ExtendedMetadataContainer(type: typeof(double)),
            new ExtendedMetadataContainer(type: typeof(float)),
            new ExtendedMetadataContainer(type: typeof(bool)),
            new ExtendedMetadataContainer(type: typeof(DateTime)),
            new ExtendedMetadataContainer(type: typeof(DateTimeOffset)),
            new ExtendedMetadataContainer(type: typeof(TimeSpan)),
            new ExtendedMetadataContainer(type: typeof(IEnumerable<object>)),
            new ExtendedMetadataContainer(type: typeof(ICollection<object>)),
            new ExtendedMetadataContainer(type: typeof(IDictionary<string, object>)),
            new ExtendedMetadataContainer(type: typeof(object)),
            new ExtendedMetadataContainer(type: typeof(Guid))
        }.Select(selector: type =>
        {
            type.Category = "System";
            return type;
        })
            .ToArray();

        return metadataContainerSet;
    }

    private static ExtendedMetadataContainer Entity<T>() =>
        CreateExtendedMetadataContainer(
            type: typeof(T),
            category: "ContentManagement",
            isEntity: true,
            hasEndpoint: true);

    private static ExtendedMetadataContainer Complex<T>() =>
        CreateExtendedMetadataContainer(
            type: typeof(T),
            category: "ContentManagement");

    private static ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        string category,
        bool isEntity = false,
        bool hasEndpoint = false) =>
        new ExtendedMetadataContainer(type: type, isEntity: isEntity, hasEndpoint: hasEndpoint)
        {
            Category = category
        };

}