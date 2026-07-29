// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Testing;
using Web.AcceptanceTests.Models;

namespace Web.AcceptanceTests.Infrastructure;

internal static class AcceptanceConfiguration
{
    internal static AcceptanceSettings CreateSettings()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        return new AcceptanceSettings
        {
            CoreConnectionString =
                configuration.ContentManagementConnectionString,
            SsoConnectionString = configuration.SecurityConnectionString,
            DecryptionKey = configuration.SecurityDecryptionKey,
        };
    }
}