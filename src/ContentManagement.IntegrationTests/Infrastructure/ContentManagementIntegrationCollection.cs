// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace ContentManagement.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class ContentManagementIntegrationCollection
    : ICollectionFixture<ContentManagementIntegrationFixture>
{
    public const string Name = "ContentManagement integration";
}