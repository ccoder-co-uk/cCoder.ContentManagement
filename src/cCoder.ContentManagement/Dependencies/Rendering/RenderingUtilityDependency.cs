// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Security.Cryptography;
using System.Text;
using cCoder.ContentManagement.Models.Rendering;

namespace cCoder.ContentManagement.Dependencies.Rendering;

internal sealed class RenderingUtilityDependency(
    SystemTextJsonDependency jsonDependency = null)
{
    private readonly SystemTextJsonDependency jsonDependency =
        jsonDependency ?? new SystemTextJsonDependency();

    public string HtmlEncode(string value) =>
        WebUtility.HtmlEncode(value: value);

    public string Serialize(object value) =>
        jsonDependency.Serialize(value: value);

    public RuntimePropertyValue[] GetPropertyValues(object value) =>
        value.GetType()
            .GetProperties()
            .Select(selector: property => new RuntimePropertyValue
            {
                Name = property.Name,
                Value = property.GetValue(obj: value),
                IsValueType = property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string)
            })
            .ToArray();

    public string ComputeFingerprint(object value) =>
        Convert.ToHexString(
            inArray: SHA256.HashData(
                source: Encoding.UTF8.GetBytes(
                    s: jsonDependency.Serialize(value: value))));
}