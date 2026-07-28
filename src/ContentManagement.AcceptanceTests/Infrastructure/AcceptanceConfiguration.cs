// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Web.AcceptanceTests.Models;

namespace Web.AcceptanceTests.Infrastructure;

internal static class AcceptanceConfiguration
{
    internal static AcceptanceSettings CreateSettings() =>
        new()
        {
            CoreConnectionString = AddDatabaseSuffix(
                variableName: "ContentManagement__ConnectionString"),
            SsoConnectionString = AddDatabaseSuffix(
                variableName: "Security__ConnectionString"),
            DecryptionKey =
                ReadEnvironmentVariable(
                    variableName: "Security__DecryptionKey")
                ?? "000000000000000000000000000000000000000000000000",
        };

    private static string AddDatabaseSuffix(string variableName)
    {
        string connectionString =
            ReadEnvironmentVariable(variableName: variableName)
            ?? ReadConfiguredConnectionString(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(
            connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            return connectionString;
        }

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";

        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(
        string variableName)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(
                path: "appsettings.testing.json",
                optional: true)
            .Build();

        return configuration[variableName.Replace(
            oldValue: "__",
            newValue: ":")] ?? string.Empty;
    }

    private static string ReadEnvironmentVariable(string variableName) =>
        Environment.GetEnvironmentVariable(variable: variableName)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.User)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.Machine);
}