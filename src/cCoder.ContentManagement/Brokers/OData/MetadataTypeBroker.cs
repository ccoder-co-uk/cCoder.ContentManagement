// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using System.Reflection;

namespace cCoder.ContentManagement.Brokers.OData;

internal sealed class MetadataTypeBroker : IMetadataTypeBroker
{
    public T GetCustomAttribute<T>(MemberInfo memberInfo)
        where T : Attribute =>
        memberInfo.GetCustomAttribute<T>();

    public string Serialize(object value) =>
        JsonSerializer.Serialize(value: value);
}