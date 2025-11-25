using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ElevatorApi.Tests;

public class ElevatorRequestsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ElevatorRequestsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RequestElevator_ToCurrentFloor_ReturnsAcceptedWithEchoedFloor()
    {
        // Arrange
        var payload = new RequestElevatorDto(Floor: 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/elevator/request", payload);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RequestElevatorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(5, body!.RequestedFloor);
        Assert.Equal("queued", body.Status);
    }

    // Local DTOs for contract verification
    private sealed record RequestElevatorDto(int Floor);
    private sealed record RequestElevatorResponseDto(int RequestedFloor, string Status);
}
