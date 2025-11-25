using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ElevatorApi.Tests;

public class DestinationRequestsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DestinationRequestsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RequestDestinationFloor_ReturnsAcceptedWithEchoedFloor()
    {
        // Arrange
        var payload = new RequestDestinationDto(Floor: 10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/elevator/destination", payload);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RequestDestinationResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(10, body!.DestinationFloor);
        Assert.Equal("queued", body.Status);
    }

    private sealed record RequestDestinationDto(int Floor);
    private sealed record RequestDestinationResponseDto(int DestinationFloor, string Status);
}
