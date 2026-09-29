// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using LocalPageRole = cCoder.Data.Models.Security.PageRole;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenItemUsesCompositeKeyForDeleteAllAsync()
    {
        // Given
        LocalPageRole link = new() { PageId = Random.Shared.Next(minValue: 1, maxValue: 1000), RoleId = Guid.NewGuid() };

        // When
        Func<Task> act = async () => await pageRoleProcessingService.DeleteAllPageRoleAsync(deletedPageRole: new[] { link });

        // Then

        await act.Should()
            .ThrowAsync<System.Security.SecurityException>();
    }

}