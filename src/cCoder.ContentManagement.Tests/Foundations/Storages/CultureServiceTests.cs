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

public partial class CultureServiceTests
{
    private readonly Mock<ICultureBroker> cultureBrokerMock;
    private readonly CultureService cultureService;

    public CultureServiceTests()
    {
        cultureBrokerMock = new Mock<ICultureBroker>(behavior: MockBehavior.Strict);
        cultureService = new CultureService(
cultureBroker: cultureBrokerMock.Object
        );
    }

    private static Culture CreateRandomCulture(string id = null)
    {
        Culture culture = Builder<Culture>
            .CreateNew()
            .With(func: x => x.Id = id ?? $"culture-{Guid.NewGuid():N}")
            .With(func: x => x.Name = $"Culture-{Guid.NewGuid():N}")
            .Build();

        return culture;
    }
}