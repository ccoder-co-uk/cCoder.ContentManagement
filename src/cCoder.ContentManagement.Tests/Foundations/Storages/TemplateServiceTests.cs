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

public partial class TemplateServiceTests
{
    private readonly Mock<ITemplateBroker> templateBrokerMock;
    private readonly TemplateService templateService;

    public TemplateServiceTests()
    {
        templateBrokerMock = new Mock<ITemplateBroker>(behavior: MockBehavior.Strict);
        templateService = new TemplateService(
templateBroker: templateBrokerMock.Object
        );
    }

    private static Template CreateRandomTemplate(int id = 42)
    {
        Template template = Builder<Template>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.Name = $"Template-{Guid.NewGuid():N}")
            .With(func: x => x.ResourceKey = $"resource-{Guid.NewGuid():N}")
            .With(func: x => x.RawString = "<html></html>")
            .With(func: x => x.AppId = 7)
            .With(func: x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return template;
    }
}