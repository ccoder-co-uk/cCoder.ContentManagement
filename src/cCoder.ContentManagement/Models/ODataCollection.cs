// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json.Serialization;

namespace cCoder.ContentManagement.Models;

public class ODataCollection<TCollectionType>
{
    [JsonPropertyName("@odata.context")]
    public string ODataContext { get; set; }

    public IEnumerable<TCollectionType> Value { get; set; }
}