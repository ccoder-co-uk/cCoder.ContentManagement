// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using Microsoft.Data.SqlClient;

namespace ContentManagement.IntegrationTests.Infrastructure;

internal sealed class IntegrationTestConfiguration
{
    private IntegrationTestConfiguration(
        string contentManagementConnectionString,
        string securityConnectionString,
        string securityDecryptionKey)
    {
        ContentManagementConnectionString = contentManagementConnectionString;
        SecurityConnectionString = securityConnectionString;
        SecurityDecryptionKey = securityDecryptionKey;
    }

    internal string ContentManagementConnectionString { get; }

    internal string SecurityConnectionString { get; }

    internal string SecurityDecryptionKey { get; }

    internal static IntegrationTestConfiguration Load()
    {
        string suffix = $"-integration-{Guid.NewGuid():N}";

        return new IntegrationTestConfiguration(
            contentManagementConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "CoreData__ConnectionString"),
                suffix: suffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "SecurityData__ConnectionString"),
                suffix: suffix),
            securityDecryptionKey: ReadRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: connectionString);

        if (builder.Encrypt)
        {
            builder.TrustServerCertificate = true;
        }

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Integration test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{builder.InitialCatalog}{suffix}";
        return builder.ConnectionString;
    }

    private static string ReadRequiredValue(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }
}