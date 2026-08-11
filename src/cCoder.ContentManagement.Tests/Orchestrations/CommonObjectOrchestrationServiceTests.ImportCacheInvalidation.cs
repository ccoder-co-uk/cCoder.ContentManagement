// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseOneImportedEventAfterSuccessfulRenderObjectImportAsync()
    {
        // Given
        CommonObject importedObject = CreateRandomCommonObject();
        importedObject.Type = "ContentManagement/Component";

        OperationResult<CommonObject>[] expectedResults =
        [
            new()
            {
                Success = true,
                Item = importedObject
            }
        ];

        Mock<ICommonObjectProcessingService> processingService =
            new(behavior: MockBehavior.Strict);

        Mock<ICommonObjectEventProcessingService> eventService = new();

        processingService
            .Setup(expression: service =>
                service.AddAllCommonObjectsAsync(
                    newCommonObjects: It.IsAny<CommonObject[]>()))
            .ReturnsAsync(value: expectedResults);

        CommonObjectOrchestrationService service = new(
            processingService: processingService.Object,
            eventService: eventService.Object);

        // When
        IEnumerable<OperationResult<CommonObject>> results =
            await service.AddAllCommonObjectsAsync(
                newCommonObjects: [importedObject]);

        // Then
        results.Should()
            .BeSameAs(expected: expectedResults);

        eventService.Verify(
            expression: item => item.RaiseCommonObjectsImportedEventAsync(
                commonObjects: It.Is<CommonObject[]>(items =>
                    items.Length == 1
                    && ReferenceEquals(
                        objA: items[0],
                        objB: importedObject))),
            times: Times.Once());

        processingService.VerifyAll();
    }
}
