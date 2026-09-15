// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using cCoder.ContentManagement.Models.OData;

namespace cCoder.ContentManagement.Dependencies.OData;

internal sealed class MetadataTypeDependency(SystemTextJsonDependency jsonDependency = null)
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

    private readonly SystemTextJsonDependency jsonDependency =
        jsonDependency ?? new SystemTextJsonDependency();

    public MetadataTypeDefinition GetDefinition<T>() => GetDefinition(type: typeof(T));

    public MetadataTypeDefinition GetDefinition(Type type) => CreateDefinition(type: type);

    public string Serialize(object value) => jsonDependency.Serialize(value: value);

    private static MetadataTypeDefinition CreateDefinition(Type type)
    {
        bool isValueType = type.IsValueType || type == typeof(string);

        return new MetadataTypeDefinition
        {
            IsValueType = isValueType,
            Type = GetMetadataTypeName(type: type),
            Name = type.Name,
            ServerType = type.AssemblyQualifiedName,
            ServerTypeName = GetCSharpTypeName(type: type),
            Properties = isValueType ? [] : type.GetProperties()
                .Select(selector: CreatePropertyDefinition).ToArray(),
            IsJoinEntity = IsJoinType(type: type)
        };
    }

    private static MetadataPropertyDefinition CreatePropertyDefinition(PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetMetadataTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = GetCSharpTypeName(type: property.PropertyType),
            IsValueType = property.PropertyType.IsValueType || property.PropertyType == typeof(string),
            IsReadOnly = !property.CanWrite,
            IsKey = property.GetCustomAttribute<KeyAttribute>() is not null || property.Name == "Id",
            IsRequired = (!(property.PropertyType.IsGenericType
                && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || property.GetCustomAttribute<RequiredAttribute>() is not null
        };

    private static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType) return type.Name;

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(selector: GetCSharpTypeName);
        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>"
            .Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    private static string GetMetadataTypeName(Type type)
    {
        if (type == typeof(string)) return "string";
        if (typeof(IEnumerable).IsAssignableFrom(c: type)) return "array";
        return TypeNames.TryGetValue(key: type, value: out string name) ? name : "object";
    }

    private static bool IsJoinType(Type type)
    {
        TableAttribute table = type.GetCustomAttribute<TableAttribute>();
        return table != null && type.GetProperties().Length == 4
            && type.GetProperties()
                .Where(predicate: property => property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string))
                .All(predicate: property => property.GetCustomAttribute<ForeignKeyAttribute>() != null);
    }
}