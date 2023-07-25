using Checkers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(Options =>
{
    Options.SerializerOptions.Converters.Add(new PieceArrayConverter());
    Options.SerializerOptions.Converters.Add(new MovementConverter());
    Options.SerializerOptions.Converters.Add(new CoordinateConverter());
    Options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

var corsPolicy = "_AllowAngularApp";

builder.Services.AddCors(Options =>
{
    Options.AddPolicy(name: corsPolicy,
    policy =>
    {
        policy.WithOrigins("http://localhost:4200");
    });
});

WebApplication app = builder.Build();

app.UseCors(corsPolicy);

var checkers = app.MapGroup("/checkers");

var board = new Board();

checkers.MapGet("/new", NewBoard);
checkers.MapPost("/valid-movements", GetValidMovements);
checkers.MapPost("/move", ApplyMovement);
checkers.MapPost("/end-turn", EndTurn);

app.Run();

IResult NewBoard()
{
    board = new Board();
    return TypedResults.Ok(board);
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
    Console.WriteLine("BAD REQUEST ERROR");
    Console.WriteLine(ex.ToString());
    return TypedResults.BadRequest();
}
