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

public sealed class CommonObjectCoordinationServiceTests
{
    [Fact]
    public async Task ShouldPersistImportBeforeRaisingCompletionEventAsync()
    {
        CommonObject item = new() { Id = 1 };
        CommonObject[] items = [item];
        OperationResult<CommonObject>[] results = [new() { Success = true, Item = item }];
        Mock<ICommonObjectOrchestrationService> dataService = new(MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(MockBehavior.Strict);
        MockSequence sequence = new();
        dataService.InSequence(sequence).Setup(service => service.AddAllCommonObjectsAsync(items))
            .ReturnsAsync(results);
        eventService.InSequence(sequence).Setup(service => service.RaiseCommonObjectsImportedEventAsync(results))
            .Returns(ValueTask.CompletedTask);
        CommonObjectCoordinationService service = new(dataService.Object, eventService.Object);

        IEnumerable<OperationResult<CommonObject>> actual =
            await service.AddAllCommonObjectsAsync(items);

        actual.Should().BeSameAs(results);
        dataService.VerifyAll();
        eventService.VerifyAll();
    }

    [Fact]
    public async Task ShouldLoadRaiseDeleteEventThenDeleteAsync()
    {
        const int id = 42;
        CommonObject item = new() { Id = id };
        Mock<ICommonObjectOrchestrationService> dataService = new(MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(MockBehavior.Strict);
        MockSequence sequence = new();
        dataService.InSequence(sequence).Setup(service => service.GetCommonObject(id)).Returns(item);
        eventService.InSequence(sequence).Setup(service => service.RaiseCommonObjectDeletedEventAsync(item))
            .Returns(ValueTask.CompletedTask);
        dataService.InSequence(sequence).Setup(service => service.DeleteAsync(id))
            .Returns(ValueTask.CompletedTask);
        CommonObjectCoordinationService service = new(dataService.Object, eventService.Object);

        await service.DeleteAsync(id);

        dataService.VerifyAll();
        eventService.VerifyAll();
    }

    [Fact]
    public async Task ShouldPersistUpdateBeforeRaisingUpdatedEventAsync()
    {
        CommonObject item = new() { Id = 42 };
        Mock<ICommonObjectOrchestrationService> dataService = new(MockBehavior.Strict);
        Mock<ICommonObjectEventOrchestrationService> eventService = new(MockBehavior.Strict);
        MockSequence sequence = new();
        dataService.InSequence(sequence).Setup(service => service.UpdateCommonObjectAsync(item))
            .ReturnsAsync(item);
        eventService.InSequence(sequence).Setup(service => service.RaiseCommonObjectUpdatedEventAsync(item))
            .Returns(ValueTask.CompletedTask);
        CommonObjectCoordinationService service = new(dataService.Object, eventService.Object);

        CommonObject actual = await service.UpdateCommonObjectAsync(item);

        actual.Should().BeSameAs(item);
        dataService.VerifyAll();
        eventService.VerifyAll();
    }
}