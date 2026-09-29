// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Storages;



using cCoder.ContentManagement.Services.Foundations.Storages;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageServiceTests
{
    private readonly Mock<IPageBroker> pageBrokerMock;
    private readonly PageService pageService;

    public PageServiceTests()
    {
        pageBrokerMock = new Mock<IPageBroker>(behavior: MockBehavior.Strict);
        pageService = new PageService(pageBroker: pageBrokerMock.Object);
    }

    private static Page CreateRandomPage(int id = 42)
    {
        Page page = Builder<Page>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = 7)
            .With(func: x => x.Order = 1)
            .With(func: x => x.ShowOnMenus = true)
            .With(func: x => x.Name = $"Page-{Guid.NewGuid():N}")
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.Path = $"/page-{Guid.NewGuid():N}")
            .With(func: x => x.ResourceKey = $"resource-{Guid.NewGuid():N}")
            .With(func: x => x.Layout = "Default")
            .Build();

        return page;
    }
}