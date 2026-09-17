// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Models.Rendering;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace cCoder.ContentManagement.Brokers.Rendering;

internal sealed class RenderingUtilityBroker : IRenderingUtilityBroker, IUtilityBroker
{
    public string HtmlEncode(string value) =>
        WebUtility.HtmlEncode(value: value);

    public string Serialize(object value) =>
        JsonSerializer.Serialize(value: value);

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
                    s: JsonSerializer.Serialize(value: value))));
}