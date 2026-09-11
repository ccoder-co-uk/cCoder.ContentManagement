// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Brokers;

internal interface ISystemTextJsonBroker
{
    T Deserialize<T>(string json);

    string Serialize(object value);

    JsonRecordsDocument ParseRecords(string json);

    JsonRecordsDocument ParseRecords(object payload);

    JsonValueDocument Normalize(object value);
}