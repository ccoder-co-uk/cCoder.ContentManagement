// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenItemUsesCompositeKeyForDeleteAllAsync()
    {
        // Given
        AppCulture link = CreateRandomAppCulture();

        // When

        Func<Task> act = async () =>
            await appCultureProcessingService.DeleteAllAppCultureAsync(deletedAppCulture: new[] { link });

        // Then

        await act.Should()
            .ThrowAsync<InvalidOperationException>();
    }

}