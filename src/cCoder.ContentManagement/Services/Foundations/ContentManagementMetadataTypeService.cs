// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Brokers.OData;

namespace cCoder.ContentManagement.Services.Foundations;

internal sealed partial class ContentManagementMetadataTypeService(
    IMetadataTypeBroker metadataTypeBroker) : IContentManagementMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch<IEnumerable<MetadataContainerSet>>(operation: () =>
            GetKnownMetadataCore());

    public IEnumerable<string> GetKnownMetadataPayloads() =>
        TryCatch<IEnumerable<string>>(operation: () =>
            GetKnownMetadataCore()
                .Select(selector: metadataTypeBroker.Serialize)
                .ToArray());

    private IEnumerable<MetadataContainerSet> GetKnownMetadataCore() =>
        new MetadataContainerSet[2]
        {
            ContentManagementTypes(),
            SystemTypes()
        }.OrderBy(keySelector: (MetadataContainerSet set) => set.Name)
            .ToArray();

    private MetadataContainerSet ContentManagementTypes()
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

    private MetadataContainerSet SystemTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "System";

        metadataContainerSet.Types = new ExtendedMetadataContainer[14]
        {
            CreateExtendedMetadataContainer<int>(category: "System"),
            CreateExtendedMetadataContainer<string>(category: "System"),
            CreateExtendedMetadataContainer<decimal>(category: "System"),
            CreateExtendedMetadataContainer<double>(category: "System"),
            CreateExtendedMetadataContainer<float>(category: "System"),
            CreateExtendedMetadataContainer<bool>(category: "System"),
            CreateExtendedMetadataContainer<DateTime>(category: "System"),
            CreateExtendedMetadataContainer<DateTimeOffset>(category: "System"),
            CreateExtendedMetadataContainer<TimeSpan>(category: "System"),
            CreateExtendedMetadataContainer<IEnumerable<object>>(category: "System"),
            CreateExtendedMetadataContainer<ICollection<object>>(category: "System"),
            CreateExtendedMetadataContainer<IDictionary<string, object>>(category: "System"),
            CreateExtendedMetadataContainer<object>(category: "System"),
            CreateExtendedMetadataContainer<Guid>(category: "System")
        }.Select(selector: type =>
        {
            type.Category = "System";
            return type;
        })
            .ToArray();

        return metadataContainerSet;
    }

    private ExtendedMetadataContainer Entity<T>() =>
        CreateExtendedMetadataContainer<T>(
            category: "ContentManagement",
            isEntity: true,
            hasEndpoint: true);

    private ExtendedMetadataContainer Complex<T>() =>
        CreateExtendedMetadataContainer<T>(
            category: "ContentManagement");

    private ExtendedMetadataContainer CreateExtendedMetadataContainer<T>(
        string category,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        MetadataTypeDefinition definition = metadataTypeBroker.GetDefinition<T>();

        return new ExtendedMetadataContainer
        {
            IsValueType = definition.IsValueType,
            Type = definition.Type,
            Name = definition.Name,
            DisplayName = definition.Name,
            Description = definition.Name,
            ServerType = definition.ServerType,
            ServerTypeName = definition.ServerTypeName,
            Properties = definition.Properties
                .Select(selector: property => new PropertyContainer
                {
                    Name = property.Name,
                    Type = property.Type,
                    ServerType = property.ServerType,
                    ServerTypeName = property.ServerTypeName,
                    IsValueType = property.IsValueType,
                    DisplayName = property.Name,
                    ShortDisplayName = property.Name,
                    Description = property.Name,
                    IsReadOnly = property.IsReadOnly,
                    Template = property.IsKey ? "key" : property.Name,
                    IsRequired = property.IsRequired
                })
                .ToArray(),
            IsEntity = isEntity,
            IsJoinEntity = isEntity && definition.IsJoinEntity,
            HasEndpoint = hasEndpoint,
            Category = category
        };
    }

}