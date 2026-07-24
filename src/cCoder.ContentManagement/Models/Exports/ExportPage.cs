// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Models.Exports;

internal sealed class ExportPage
{
    public int Id { get; init; }

    public int? ParentId { get; init; }

    public string Path { get; set; }

    public string Name { get; init; }

    public string ResourceKey { get; init; }

    public bool ShowOnMenus { get; init; }

    public int Order { get; init; }

    public DateTimeOffset LastUpdated { get; init; }

    public string Layout { get; init; }

    public ExportContent[] Contents { get; init; }

    public ExportPageInfo[] PageInfo { get; init; }
}