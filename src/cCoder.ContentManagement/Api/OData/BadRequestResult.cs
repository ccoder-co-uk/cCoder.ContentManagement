using cCoder.ContentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.ContentManagement.Api.OData;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(modelState)
    {
        base.Value = JsonConvert.SerializeObject(modelState.Select<KeyValuePair<string, ModelStateEntry>, ModelStateError>((KeyValuePair<string, ModelStateEntry> item) => new ModelStateError
        {
            Key = item.Key,
            Value = item.Value?.RawValue,
            Errors = item.Value?.Errors?.Select((ModelError error) => error.ErrorMessage + " - " + error.Exception?.Message).ToArray()
        }).ToArray(), Formatting.None, new JsonSerializerSettings
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
    }
}
