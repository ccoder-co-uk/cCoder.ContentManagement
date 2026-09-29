// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class LayoutProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ILayoutService> layoutServiceMock = new();
    private readonly LayoutProcessingService layoutProcessingService;

    public LayoutProcessingServiceTests()
    {
        layoutProcessingService = new LayoutProcessingService(service: layoutServiceMock.Object);
    }

    private static Layout CreateRandomLayout() =>
        Builder<Layout>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.AppId = 1)
        .With(func: x => x.Name = $"Layout-{Guid.NewGuid():N}")
        .Build();
}