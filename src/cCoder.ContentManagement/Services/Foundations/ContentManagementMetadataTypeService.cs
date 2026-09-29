// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Brokers.OData;
using cCoder.ContentManagement.Models.OData;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace cCoder.ContentManagement.Services.Foundations;

internal sealed partial class ContentManagementMetadataTypeService(
    IMetadataTypeBroker metadataTypeBroker) : IContentManagementMetadataTypeService
{
    private static readonly Dictionary<Type, string> TypeNames = new()
    {
        { typeof(short), "number" }, { typeof(int), "number" }, { typeof(long), "number" },
        { typeof(short?), "number" }, { typeof(int?), "number" }, { typeof(long?), "number" },
        { typeof(ushort), "number" }, { typeof(uint), "number" }, { typeof(ulong), "number" },
        { typeof(ushort?), "number" }, { typeof(uint?), "number" }, { typeof(ulong?), "number" },
        { typeof(byte), "number" }, { typeof(byte?), "number" },
        { typeof(decimal), "number" }, { typeof(decimal?), "number" },
        { typeof(string), "string" }, { typeof(DateTime), "date" }, { typeof(DateTime?), "date" },
        { typeof(TimeSpan), "time" }, { typeof(TimeSpan?), "time" },
        { typeof(DateTimeOffset), "date" }, { typeof(DateTimeOffset?), "date" },
        { typeof(Guid), "guid" }, { typeof(Guid?), "guid" },
        { typeof(bool), "bool" }, { typeof(bool?), "bool" },
        { typeof(double), "number" }, { typeof(double?), "number" },
        { typeof(float), "number" }, { typeof(float?), "number" }
    };
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch<IEnumerable<MetadataContainerSet>>(operation: () =>
            GetKnownMetadataCore());

    public IEnumerable<string> GetKnownMetadataPayloads() =>
        TryCatch<IEnumerable<string>>(operation: () =>
        [
            metadataTypeBroker.Serialize(value: ContentManagementTypes()),
            metadataTypeBroker.Serialize(value: SystemTypes())
        ]);

    private IEnumerable<MetadataContainerSet> GetKnownMetadataCore()
    {
        MetadataContainerSet[] sets =
        {
            ContentManagementTypes(),
            SystemTypes()
        };

        Array.Sort(
            array: sets,
            comparison: (left, right) => string.Compare(
                strA: left.Name,
                strB: right.Name,
                comparisonType: StringComparison.Ordinal));

        return sets;
    }

    private MetadataContainerSet ContentManagementTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "ContentManagement";
        metadataContainerSet.UriBase = "ContentManagement";

        ExtendedMetadataContainer[] types =
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
        };

        Array.Sort(
            array: types,
            comparison: (left, right) => string.Compare(
                strA: left.Name,
                strB: right.Name,
                comparisonType: StringComparison.Ordinal));

        metadataContainerSet.Types = types;

        return metadataContainerSet;
    }

    private MetadataContainerSet SystemTypes()
    {
        MetadataContainerSet metadataContainerSet = new MetadataContainerSet();
        metadataContainerSet.Name = "System";

        metadataContainerSet.Types =
        [
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
        ];

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
        MetadataTypeDefinition definition = CreateMetadataTypeDefinition(
            type: typeof(T));

        List<PropertyContainer> properties = [];

        foreach (MetadataPropertyDefinition property in definition.Properties)
        {
            properties.Add(item: new PropertyContainer
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
            });
        }

        return new ExtendedMetadataContainer
        {
            IsValueType = definition.IsValueType,
            Type = definition.Type,
            Name = definition.Name,
            DisplayName = definition.Name,
            Description = definition.Name,
            ServerType = definition.ServerType,
            ServerTypeName = definition.ServerTypeName,
            Properties = [.. properties],
            IsEntity = isEntity,
            IsJoinEntity = isEntity && definition.IsJoinEntity,
            HasEndpoint = hasEndpoint,
            Category = category
        };
    }

    internal MetadataTypeDefinition GetDefinition(Type type) =>
        CreateMetadataTypeDefinition(type: type);

    private MetadataTypeDefinition CreateMetadataTypeDefinition(Type type)
    {
        bool isValueType = type.IsValueType || type == typeof(string);

        List<MetadataPropertyDefinition> properties = [];

        if (!isValueType)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                properties.Add(item: CreateMetadataPropertyDefinition(
                    property: property));
            }
        }

        return new MetadataTypeDefinition
        {
            IsValueType = isValueType,
            Type = GetMetadataTypeName(type: type),
            Name = type.Name,
            ServerType = type.AssemblyQualifiedName,
            ServerTypeName = GetCSharpTypeName(type: type),
            Properties = [.. properties],
            IsJoinEntity = IsJoinType(type: type)
        };
    }

    private MetadataPropertyDefinition CreateMetadataPropertyDefinition(
        PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetMetadataTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = GetCSharpTypeName(type: property.PropertyType),
            IsValueType = property.PropertyType.IsValueType || property.PropertyType == typeof(string),
            IsReadOnly = !property.CanWrite,
            IsKey = metadataTypeBroker.GetCustomAttribute<KeyAttribute>(
                memberInfo: property) is not null || property.Name == "Id",
            IsRequired = (!(property.PropertyType.IsGenericType
                && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || metadataTypeBroker.GetCustomAttribute<RequiredAttribute>(
                    memberInfo: property) is not null
        };

    private static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        List<string> genericNames = [];

        foreach (Type genericType in type.GenericTypeArguments)
        {
            genericNames.Add(item: GetCSharpTypeName(type: genericType));
        }

        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>"
            .Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    private static string GetMetadataTypeName(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }

        if (typeof(IEnumerable).IsAssignableFrom(c: type))
        {
            return "array";
        }

        return TypeNames.TryGetValue(key: type, value: out string name)
            ? name
            : "object";
    }

    private bool IsJoinType(Type type)
    {
        TableAttribute table = metadataTypeBroker
            .GetCustomAttribute<TableAttribute>(memberInfo: type);

        PropertyInfo[] properties = type.GetProperties();

        if (table is null || properties.Length != 4)
        {
            return false;
        }

        foreach (PropertyInfo property in properties)
        {
            bool isValueProperty = property.PropertyType.IsValueType
                || property.PropertyType == typeof(string);

            if (isValueProperty && metadataTypeBroker
                .GetCustomAttribute<ForeignKeyAttribute>(
                    memberInfo: property) is null)
            {
                return false;
            }
        }

        return true;
    }

}