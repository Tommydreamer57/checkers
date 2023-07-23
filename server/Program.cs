using Checkers;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(Options =>
{
    Options.SerializerOptions.Converters.Add(new PieceArrayConverter());
    Options.SerializerOptions.Converters.Add(new MovementConverter());
    Options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

WebApplication app = builder.Build();

var checkers = app.MapGroup("/checkers");

var board = new Board();

checkers.MapGet("/new", NewBoard);
checkers.MapPost("/move", ApplyMovement);
checkers.MapPost("end-turn", EndTurn);

app.Run();

IResult NewBoard()
{
    board = new Board();
    return TypedResults.Ok(board);
}

IResult ApplyMovement(Movement movement)
{
    try
    {
        board.ApplyMovement(movement);

        return TypedResults.Ok(board);
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

        return TypedResults.Ok(board);
    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }
}

IResult BadRequest(Exception ex)
{
    Console.WriteLine(ex.ToString());
    return TypedResults.BadRequest();
}
