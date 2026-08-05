// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Authorization;
using cCoder.ContentManagement.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AuthorizationProcessingServiceTests
{
    private readonly Mock<IAuthorizationService> authorizationServiceMock;
    private readonly AuthorizationProcessingService processingService;

    public AuthorizationProcessingServiceTests()
    {
        authorizationServiceMock = new Mock<IAuthorizationService>(
            behavior: MockBehavior.Strict);

        processingService = new AuthorizationProcessingService(
            authorizationService: authorizationServiceMock.Object);
    }
}