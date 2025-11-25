using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ElevatorApi.Tests;

public class StopsRequestsTests
{
    [Fact]
    public async Task GetStops_AfterDestinationRequests_ReturnsSortedUniqueStops()
    {
        // New factory per test to avoid shared state across other tests
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Arrange: simulate current passengers requesting destinations
        await client.PostAsJsonAsync("/api/elevator/destination", new { floor = 10 });
        await client.PostAsJsonAsync("/api/elevator/destination", new { floor = 3 });
        await client.PostAsJsonAsync("/api/elevator/destination", new { floor = 10 }); // duplicate

        // Act
        var response = await client.GetAsync("/api/elevator/stops");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StopsResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(new[] { 3, 10 }, body!.Stops);
    }

    private sealed record StopsResponseDto(int[] Stops);
}
