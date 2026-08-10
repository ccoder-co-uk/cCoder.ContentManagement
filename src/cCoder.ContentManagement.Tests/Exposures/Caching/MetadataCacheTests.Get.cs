// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies.Caching;
using cCoder.ContentManagement.Models.OData;
using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Exposures.Caching;

public partial class MetadataCacheTests
{
    [Fact]
    public void Get_ShouldResolveCultureWithoutCaseSensitivity()
    {
        // Given
        MetadataCacheDependency subject = CreateSubject(
            typeSets:
            [
                new MetadataContainerSet
                {
                    Name = "Core",
                    Types = []
                }
            ]);

        // When
        string result = subject.Get(
            key: "core",
            culture: "en-gb");

        // Then
        result.Should()
            .NotBeNullOrWhiteSpace();
    }
}