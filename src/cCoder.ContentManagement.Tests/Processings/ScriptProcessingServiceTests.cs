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

public partial class ScriptProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IScriptService> scriptServiceMock = new();
    private readonly ScriptProcessingService scriptProcessingService;

    public ScriptProcessingServiceTests()
    {
        scriptProcessingService = new ScriptProcessingService(service: scriptServiceMock.Object);
    }

    private static Script CreateRandomScript() =>
        Builder<Script>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.AppId = 1)
        .With(func: x => x.Name = $"Script-{Guid.NewGuid():N}")
        .Build();
}