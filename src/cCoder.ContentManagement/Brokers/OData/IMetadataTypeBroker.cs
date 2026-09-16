// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.OData;

namespace cCoder.ContentManagement.Brokers.OData;

using System.Reflection;

internal interface IMetadataTypeBroker
{
    T GetCustomAttribute<T>(MemberInfo memberInfo)
        where T : Attribute;

    string Serialize(object value);
}