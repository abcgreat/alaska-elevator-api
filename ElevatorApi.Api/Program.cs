var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/api/elevator/request", (RequestElevatorDto dto) =>
{
    if (dto.Floor < 1)
        return Results.BadRequest(new { error = "Floor must be >= 1" });

    var response = new RequestElevatorResponseDto(
        RequestedFloor: dto.Floor,
        Status: "queued"
    );

    return Results.Accepted($"/api/elevator/requests/{dto.Floor}", response);
})
.WithName("RequestElevator")
.WithOpenApi();

app.MapPost("/api/elevator/destination", (RequestDestinationDto dto) =>
{
    if (dto.Floor < 1)
        return Results.BadRequest(new { error = "Floor must be >= 1" });

    var response = new RequestDestinationResponseDto(
        DestinationFloor: dto.Floor,
        Status: "queued"
    );

    return Results.Accepted($"/api/elevator/destinations/{dto.Floor}", response);
})
.WithName("RequestDestination")
.WithOpenApi();

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

public partial class Program { }

internal sealed record RequestElevatorDto(int Floor);
internal sealed record RequestElevatorResponseDto(int RequestedFloor, string Status);
internal sealed record RequestDestinationDto(int Floor);
internal sealed record RequestDestinationResponseDto(int DestinationFloor, string Status);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
