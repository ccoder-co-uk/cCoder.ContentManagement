// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Extensions.OData;
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

        metadataContainerSet.Types = new ExtendedMetadataContainer[23]
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
            Complex<PageRenderResult>(),
            Complex<TemplateRenderResult>(),
            Complex<ComponentRenderResult>(),
            Complex<Style>(),
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
            typeof(int).CreateExtendedMetadataContainer(),
            typeof(string).CreateExtendedMetadataContainer(),
            typeof(decimal).CreateExtendedMetadataContainer(),
            typeof(double).CreateExtendedMetadataContainer(),
            typeof(float).CreateExtendedMetadataContainer(),
            typeof(bool).CreateExtendedMetadataContainer(),
            typeof(DateTime).CreateExtendedMetadataContainer(),
            typeof(DateTimeOffset).CreateExtendedMetadataContainer(),
            typeof(TimeSpan).CreateExtendedMetadataContainer(),
            typeof(IEnumerable<object>).CreateExtendedMetadataContainer(),
            typeof(ICollection<object>).CreateExtendedMetadataContainer(),
            typeof(IDictionary<string, object>).CreateExtendedMetadataContainer(),
            typeof(object).CreateExtendedMetadataContainer(),
            typeof(Guid).CreateExtendedMetadataContainer()
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
        bool hasEndpoint = false)
    {
        ExtendedMetadataContainer metadata = type.CreateExtendedMetadataContainer(
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

        metadata.Category = category;

        return metadata;
    }

}