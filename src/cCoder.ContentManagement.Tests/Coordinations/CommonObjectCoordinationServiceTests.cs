// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public sealed partial class CommonObjectCoordinationServiceTests
{
    [Fact]
    public async Task ShouldPersistImportBeforeRaisingCompletionEventAsync()
    {
        // Given
        CommonObject item = new() { Id = 1 };
        CommonObject[] items = [item];
        OperationResult<CommonObject>[] results = [new() { Success = true, Item = item }];
        Mock<ICommonObjectOrchestrationService> dataService = new(behavior: MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(behavior: MockBehavior.Strict);
        MockSequence sequence = new();

        dataService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddAllCommonObjectsAsync(
                newCommonObjects: items))
            .ReturnsAsync(value: results);

        eventService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaiseCommonObjectsImportedEventAsync(
                results: results))
            .Returns(value: ValueTask.CompletedTask);

        CommonObjectCoordinationService service = new(
            commonObjectOrchestrationService: dataService.Object,
            eventOrchestrationService: eventService.Object);

        // When
        IEnumerable<OperationResult<CommonObject>> actual =
            await service.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        actual.Should()
            .BeSameAs(expected: results);

        dataService.VerifyAll();
        eventService.VerifyAll();
    }

    [Fact]
    public async Task ShouldLoadRaiseDeleteEventThenDeleteAsync()
    {
        // Given
        const int id = 42;
        CommonObject item = new() { Id = id };
        Mock<ICommonObjectOrchestrationService> dataService = new(behavior: MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(behavior: MockBehavior.Strict);
        MockSequence sequence = new();

        dataService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.GetCommonObject(commonObjectId: id))
            .Returns(value: item);

        eventService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaiseCommonObjectDeletedEventAsync(
                commonObject: item))
            .Returns(value: ValueTask.CompletedTask);

        dataService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.DeleteAsync(commonObjectId: id))
            .Returns(value: ValueTask.CompletedTask);

        CommonObjectCoordinationService service = new(
            commonObjectOrchestrationService: dataService.Object,
            eventOrchestrationService: eventService.Object);

        // When
        await service.DeleteAsync(commonObjectId: id);

        // Then
        dataService.VerifyAll();
        eventService.VerifyAll();
    }

    [Fact]
    public async Task ShouldPersistUpdateBeforeRaisingUpdatedEventAsync()
    {
        // Given
        CommonObject item = new() { Id = 42 };
        Mock<ICommonObjectOrchestrationService> dataService = new(behavior: MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(behavior: MockBehavior.Strict);
        MockSequence sequence = new();

        dataService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.UpdateCommonObjectAsync(
                updatedCommonObject: item))
            .ReturnsAsync(value: item);

        eventService
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaiseCommonObjectUpdatedEventAsync(
                commonObject: item))
            .Returns(value: ValueTask.CompletedTask);

        CommonObjectCoordinationService service = new(
            commonObjectOrchestrationService: dataService.Object,
            eventOrchestrationService: eventService.Object);

        // When
        CommonObject actual = await service.UpdateCommonObjectAsync(updatedCommonObject: item);

        // Then
        actual.Should()
            .BeSameAs(expected: item);

        dataService.VerifyAll();
        eventService.VerifyAll();
    }
}