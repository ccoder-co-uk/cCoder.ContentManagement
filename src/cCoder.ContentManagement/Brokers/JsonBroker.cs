// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace cCoder.ContentManagement.Brokers;

internal sealed class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(value: json);

    public T ParseJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(value: json);

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(value: value);

    public string SerializeIgnoringReferences(object value) =>
        JsonConvert.SerializeObject(
            value: value,
            formatting: Formatting.None,
            settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                },
                MaxDepth = 4
            });

    public bool IsJsonObject(object value) =>
        value is JObject;

    public bool IsJsonArray(object value) =>
        value is JArray;

    public bool IsJsonValue(object value) =>
        value is JValue;

    public IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value) =>
        ((JObject)value)
            .Properties()
            .Select(selector: property =>
                new KeyValuePair<string, object>(
                    key: property.Name,
                    value: property.Value));

    public IEnumerable<object> GetJsonItems(object value) =>
        ((JArray)value).Cast<object>();

    public void RemoveJsonProperty(object value, string propertyName) =>
        ((JObject)value)
            .Property(name: propertyName, comparison: StringComparison.OrdinalIgnoreCase)
            .Remove();
}