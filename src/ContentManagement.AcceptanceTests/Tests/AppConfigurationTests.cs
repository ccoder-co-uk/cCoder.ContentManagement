// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;
using Xunit;

namespace ContentManagement.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void ShouldExposeEveryRequiredDomainConfiguration()
    {
        // Given
        const string typeName =
            "ContentManagement.Web.Models.AppConfiguration, ContentManagement.Web";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        Assert.NotNull(@object: configurationType);

        Assert.Equal(
            expected: typeof(AppSecurityConfiguration),
            actual: configurationType.GetProperty(name: "AppSecurity")?.PropertyType);

        Assert.Equal(
            expected: typeof(ContentManagementConfiguration),
            actual: configurationType.GetProperty(name: "ContentManagement")?.PropertyType);

        Assert.Equal(
            expected: typeof(CoreDataConfiguration),
            actual: configurationType.GetProperty(name: "CoreData")?.PropertyType);

        Assert.Equal(
            expected: typeof(EventingConfiguration),
            actual: configurationType.GetProperty(name: "Eventing")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityConfiguration),
            actual: configurationType.GetProperty(name: "Security")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityDataConfiguration),
            actual: configurationType.GetProperty(name: "SecurityData")?.PropertyType);
    }
}