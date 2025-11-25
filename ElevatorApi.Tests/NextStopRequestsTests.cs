using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ElevatorApi.Tests;

public class NextStopRequestsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NextStopRequestsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetNextStop_WithQueuedStops_ReturnsLowestStop()
    {
        // Arrange: queue some stops (both destination and pickup)
        await _client.PostAsJsonAsync("/api/elevator/destination", new { floor = 10 });
        await _client.PostAsJsonAsync("/api/elevator/destination", new { floor = 3 });
        await _client.PostAsJsonAsync("/api/elevator/request",     new { floor = 7 });

        // Act
        var response = await _client.GetAsync("/api/elevator/next");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<NextStopResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(3, body!.NextStop); // smallest floor
    }

    [Fact]
    public async Task GetNextStop_WhenNoStops_ReturnsNull()
    {
        // Arrange: no stops queued

        // Act
        var response = await _client.GetAsync("/api/elevator/next");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<NextStopResponseDto>();
        Assert.NotNull(body);
        Assert.Null(body!.NextStop);
    }

    private sealed record NextStopResponseDto(int? NextStop);
}
