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

public partial class LayoutServiceTests
{
    private readonly Mock<ILayoutBroker> layoutBrokerMock;
    private readonly LayoutService layoutService;

    public LayoutServiceTests()
    {
        layoutBrokerMock = new Mock<ILayoutBroker>(behavior: MockBehavior.Strict);
        layoutService = new LayoutService(layoutBroker: layoutBrokerMock.Object);
    }

    private static Layout CreateRandomLayout(int id = 42, int appId = 7)
    {
        Layout layout = Builder<Layout>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.HeaderHtml = "<header>Header</header>")
            .With(func: x => x.Html = "<main>Layout</main>")
            .With(func: x => x.Script = "console.log('layout');")
            .With(func: x => x.Name = $"Layout-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.LastUpdatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(minutes: -5))
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return layout;
    }
}