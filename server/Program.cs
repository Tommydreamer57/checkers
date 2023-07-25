using Checkers;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var corsPolicy = "_AllowAngularApp";

builder.Services.AddCors(Options =>
{
    Options.AddPolicy(name: corsPolicy,
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(Options =>
    {
        Options.SerializerOptions.Converters.Add(new PieceArrayConverter());
        Options.SerializerOptions.Converters.Add(new MovementConverter());
        Options.SerializerOptions.Converters.Add(new CoordinateConverter());
        Options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

WebApplication app = builder.Build();

app.UseCors(corsPolicy);

var board = new Board();

var checkers = app.MapGroup("/api/checkers");

checkers.MapGet("/current", GetBoard);
checkers.MapPost("/new", NewBoard);
checkers.MapPost("/valid-movements", GetValidMovements);
checkers.MapPost("/move", ApplyMovement);
checkers.MapPost("/end-turn", EndTurn);

app.Run();

IResult GetBoard()
{
    return TypedResults.Ok(board);
}

IResult NewBoard()
{
    board = new Board();
    return GetBoard();
}

IResult GetValidMovements((int X, int Y) start)
{
    try
    {
        Console.WriteLine(start.ToString());

        var movements = board.GetValidMovements(start);

        return TypedResults.Ok(movements);
    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }
}

IResult ApplyMovement(Movement movement)
{
    try
    {
        board.ApplyMovement(movement);

        return GetBoard();
    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }
}

IResult EndTurn()
{
    try
    {
        board.SwitchTurns();

        return GetBoard();
    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }
}

IResult BadRequest(Exception? ex)
{
    Console.WriteLine("BAD REQUEST ERROR");
    if (ex != null) Console.WriteLine(ex.ToString());
    return TypedResults.BadRequest();
}