// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using Moq;

namespace cCoder.ContentManagement.Tests.Aggregations;

public partial class ContentManagementMigrationAggregationServiceTests
{
    private static ContentManagementMigrationAggregationService CreateService(
        IPackageExportProcessingService packageExportProcessingService = null) =>
        new(migrationSupportOrchestrationService:
            new MigrationSupportOrchestrationService(
                jsonProcessingService: Mock.Of<IJsonProcessingService>(),
                packageExportProcessingService:
                    packageExportProcessingService
                    ?? Mock.Of<IPackageExportProcessingService>()));
}