// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace ContentManagement.IntegrationTests.Infrastructure;

internal sealed class IntegrationConfiguration
{
    internal string CoreConnectionString { get; init; }

    internal string SsoConnectionString { get; init; }

    internal static IntegrationConfiguration Create()
    {
        IntegrationTestConfiguration configuration =
            IntegrationTestConfiguration.Load();

        return new IntegrationConfiguration
        {
            CoreConnectionString =
                configuration.ContentManagementConnectionString,
            SsoConnectionString = configuration.SecurityConnectionString,
        };
    }
}