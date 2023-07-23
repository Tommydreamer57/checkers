Program.cs
```C#
using Checkers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var checkers = app.MapGroup("/checkers");

checkers.MapGet("/new", () => new Board());

app.Run();
```

Board.cs
```C#
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Checkers
{
    public class Board
    {
        [JsonConverter(typeof(PieceArrayConverter))]
        public Piece[,] Spaces { get; private set; }
        // public List<Tuple<Tuple<int, int>, Piece>> Pieces { get { return GetAllPieces(); } }
        public PieceColor CurrentTurnColor { get; set; }
        public int CurrentTurnCount { get; set; }
        public List<Movement> CurrentTurnMovements { get; set; }
        ...
    }
}
```

Movement.cs
```C#
using System.Diagnostics;

namespace Checkers
{
    public class Movement
    {
        public Tuple<int, int> Start { get; }
        public Tuple<int, int> Delta { get; }
        public Tuple<int, int> End { get; }
        ...
    }
}
```

Piece.cs
```C#
using System.Diagnostics;

namespace Checkers
{
    public enum PieceColor
    {
        None,
        Red,
        Black
    }
    public enum PieceType
    {
        None,
        Regular,
        King
    }
    public class Piece
    {
        public PieceColor Color { get; private set; }
        public PieceType Type { get; private set; }
        ...
    }
}
```

PieceArrayConverter.cs
```C#
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkers
{
    public class PieceArrayConverter : JsonConverter<Piece[,]>
    {
        public override Piece[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization of Piece[,] is not implemented.");
        }

        public override void Write(Utf8JsonWriter writer, Piece[,]? value, JsonSerializerOptions options)
        {
            // throw new NotImplementedException("NOT IMPLEMENTED EXCEPTION");

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();

            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();

                for (int j = 0; j < value.GetLength(1); j++)
                {
                    JsonSerializer.Serialize(writer, value[i, j], options);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}
```

checkers.csproj
```
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
  </ItemGroup>

</Project>
```