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

public partial class ContentServiceTests
{
    private readonly Mock<IContentBroker> contentBrokerMock;
    private readonly ContentService contentService;

    public ContentServiceTests()
    {
        contentBrokerMock = new Mock<IContentBroker>(behavior: MockBehavior.Strict);
        contentService = new ContentService(contentBroker: contentBrokerMock.Object);
    }

    private static Content CreateRandomContent(int id = 42, int pageId = 7)
    {
        Content content = Builder<Content>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.PageId = pageId)
            .With(func: x => x.CultureId = "en-GB")
            .With(func: x => x.Name = $"Content-{Guid.NewGuid():N}")
            .With(func: x => x.Html = "<p>content</p>")
            .Build();

        return content;
    }
}