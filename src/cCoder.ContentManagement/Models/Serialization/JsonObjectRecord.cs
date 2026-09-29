// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
namespace cCoder.ContentManagement.Models.Serialization;

internal sealed class JsonObjectRecord
{
    public string RawText { get; init; }

    public IReadOnlyDictionary<string, string> StringValues { get; init; }

    public IReadOnlyDictionary<string, DateTimeOffset> DateTimeOffsetValues { get; init; }

}