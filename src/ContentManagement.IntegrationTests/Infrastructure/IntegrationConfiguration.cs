// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ContentManagement.IntegrationTests.Infrastructure;

internal sealed class IntegrationConfiguration
{
    private const string ContentManagementConnectionString =
        "ContentManagement__ConnectionString";

    private const string SecurityConnectionString =
        "Security__ConnectionString";

    internal string CoreConnectionString { get; init; }

    internal string SsoConnectionString { get; init; }

    internal static IntegrationConfiguration Create() =>
        new()
        {
            CoreConnectionString = AddDatabaseSuffix(
                connectionString: ReadSetting(
                    variableName: ContentManagementConnectionString)),
            SsoConnectionString = AddDatabaseSuffix(
                connectionString: ReadSetting(
                    variableName: SecurityConnectionString)),
        };

    private static string AddDatabaseSuffix(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(connectionString: connectionString);

        if (!string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            builder.InitialCatalog =
                $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";
        }

        return builder.ConnectionString;
    }

    private static string ReadSetting(string variableName)
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

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(
                path: "appsettings.testing.json",
                optional: true)
            .Build();

        return configuration[variableName.Replace(
            oldValue: "__",
            newValue: ":",
            comparisonType: StringComparison.Ordinal)] ?? string.Empty;
    }
}