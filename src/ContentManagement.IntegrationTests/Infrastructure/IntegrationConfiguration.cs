// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Testing;

namespace ContentManagement.IntegrationTests.Infrastructure;

internal sealed class IntegrationConfiguration
{
    internal string CoreConnectionString { get; init; }

    internal string SsoConnectionString { get; init; }

    internal static IntegrationConfiguration Create()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        return new IntegrationConfiguration
        {
            CoreConnectionString =
                configuration.ContentManagementConnectionString,
            SsoConnectionString = configuration.SecurityConnectionString,
        };
    }
}