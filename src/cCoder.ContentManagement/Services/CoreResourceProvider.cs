// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services;

internal class CoreResourceProvider : IResourceProvider
{
    private readonly IResourceProcessingService service;

    public CoreResourceProvider(IResourceProcessingService resourceService)
    {
        service = resourceService;
    }

    public Resource GetResource(string key, string culture)
    {
        string text = key.Split(separator: '.')
            .Last();

        string text2 = Regex.Replace(input: text.Replace(oldValue: "Id", newValue: ""), pattern: "(?<!_)([A-Z])", replacement: " $1");

        return new Resource
        {
            Culture = string.Empty,
            Name = text,
            Key = key,
            DisplayName = text2,
            ShortDisplayName = text2,
            Description = text2
        };
    }
}