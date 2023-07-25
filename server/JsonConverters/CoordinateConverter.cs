using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkers
{
  public class CoordinateConverter : JsonConverter<(int X, int Y)>
  {
    public override (int X, int Y) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {

      using JsonDocument doc = JsonDocument.ParseValue(ref reader);
      var root = doc.RootElement;

      if (root.ValueKind != JsonValueKind.Object ||
          !root.TryGetProperty("x", out JsonElement xElement) ||
          !root.TryGetProperty("y", out JsonElement yElement) ||
          xElement.ValueKind != JsonValueKind.Number ||
          yElement.ValueKind != JsonValueKind.Number)
      {
        throw new JsonException("Invalid coordinates in JSON.");
      }

      int x = xElement.GetInt32();
      int y = yElement.GetInt32();

      return (x, y);
    }

    public override void Write(Utf8JsonWriter writer, (int X, int Y) value, JsonSerializerOptions options)
    {
      throw new NotImplementedException("Serialization of (int X, int Y) is not implemented.");
    }
  }
}